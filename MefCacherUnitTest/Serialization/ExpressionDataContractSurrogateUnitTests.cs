using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhNoPub.MefCacher.Serialization;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Xml;

namespace OhNoPub.MefCacherUnitTest.Serialization
{
    [TestClass]
    public class ExpressionDataContractSurrogateUnitTests
    {
        [TestMethod]
        public void SerializeExpression()
        {
            Expression<Func<int>> expression = () => 4;
            var serializer = new DataContractSerializer(
                expression.GetType(),
                new DataContractSerializerSettings()
                .Apply(
                    new ExpressionDataContractSurrogate(),
                    new TypeDataContractSurrogate()));
            using (var buf = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(buf, new XmlWriterSettings { Indent = true, }))
                {
                    serializer.WriteObject(writer, expression);
                }

                buf.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(buf))
                {
                    Console.WriteLine(reader.ReadToEnd());

                    // Deserialize
                    buf.Seek(0, SeekOrigin.Begin);
                    using (var xmlReader = XmlReader.Create(buf))
                    {
                        var result = (Expression<Func<int>>)serializer.ReadObject(xmlReader);
                        Console.WriteLine(result);
                        Console.WriteLine($"->{result.Compile()()}");
                        Assert.AreEqual(4, result.Compile()());
                    }
                }
            }
        }

        [TestMethod]
        public void RoundtripArrayOfExpression()
        {
            // Use object on purpose to trigger the whole requirement for KnownTypes to work at all.
            var exprs = (Expression<Func<string>>[])new DataContractSerializerWrapper<object>(
                new DataContractSerializerSettings()
                .Apply(
                    new ExpressionDataContractSurrogate(),
                    new TypeDataContractSurrogate()))
                .Roundtrip(
                    new Expression<Func<string>>[] {
                        () => "A",
                        () => "B",
                    });
            Assert.AreEqual("A", exprs[0].Compile()());
            Assert.AreEqual("B", exprs[1].Compile()());
        }

        [TestMethod] public void RoundtripExpression_Add() => RoundTrip(a => 2 + a, 3, 5);
        [TestMethod]
        public void RoundtripExpression_AddAssign()
        {
            var p1Expression = Expression.Parameter(typeof(int));
            var v1Expression = Expression.Variable(typeof(int));
            RoundTrip(
                Expression.Lambda<Func<int, int>>(
                    Expression.Block(
                        new[] { v1Expression, },
                        Expression.AddAssign(v1Expression, p1Expression),
                        Expression.AddAssign(v1Expression, p1Expression)),
                    p1Expression),
                3,
                6);
        }
        [TestMethod]
        public void RoundtripExpression_AddAssignChecked()
        {
            var p1Expression = Expression.Parameter(typeof(int));
            var v1Expression = Expression.Variable(typeof(int));
            UsingRoundtrippedValue(
                Expression.Lambda<Func<int, int>>(
                    Expression.Block(
                        new[] { v1Expression, },
                        Expression.Assign(v1Expression, Expression.Constant(int.MaxValue)),
                        Expression.AddAssignChecked(v1Expression, p1Expression)),
                    p1Expression),
                expression =>
                {
                    var f = expression.Compile();
                    f(0);
                    {
                        var thrown = false;
                        try
                        {
                            f(1);
                        }
                        catch (OverflowException)
                        {
                            thrown = true;
                        }
                        Assert.IsTrue(thrown, "Expected checked expression.");
                    }
                });
        }

        void UsingRoundtrippedValue<T>(
            T value,
            Action<T> action)
        {
            action(
                new DataContractSerializerWrapper<T>(
                    new DataContractSerializerSettings()
                    .Apply(
                        new ExpressionDataContractSurrogate(),
                        new TypeDataContractSurrogate()))
                .Roundtrip(value));
        }

        void RoundTrip<TResult>(
            Expression<Func<TResult>> expression,
            TResult expectedResult)
        {
            UsingRoundtrippedValue(
                expression,
                e => Assert.AreEqual(
                    expectedResult,
                    e
                    .Compile()
                    ()));
        }

        void RoundTrip<T, TResult>(
            Expression<Func<T, TResult>> expression,
            T p,
            TResult expectedResult)
        {
            UsingRoundtrippedValue(
                expression,
                e => Assert.AreEqual(
                    expectedResult,
                    e
                    .Compile()
                    (p)));
        }
    }
}
