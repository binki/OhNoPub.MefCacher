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
    }
}
