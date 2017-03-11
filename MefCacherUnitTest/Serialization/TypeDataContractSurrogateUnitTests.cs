using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhNoPub.MefCacher.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OhNoPub.MefCacherUnitTest.Serialization
{
    [TestClass]
    public class TypeDataContractSurrogateUnitTests
    {
        [TestMethod]
        public void RoundtripType()
        {
            UsingSerializer(
                serializer =>
                {
                    Assert.AreEqual(typeof(int), typeof(int), $"typeof(int) should equal itself.");
                    Assert.AreEqual(
                        typeof(int),
                        serializer.Roundtrip(new List<object> { typeof(int), })[0]);
                });
        }

        [TestMethod]
        public void RoundtripArrayOfType()
        {
            UsingSerializer(
                serializer =>
                {
                    Assert.AreEqual(typeof(IEnumerable<Type[]>), typeof(IEnumerable<Type[]>));
                    Assert.AreEqual(
                        typeof(IEnumerable<Type[]>),
                        ((Type[])serializer.Roundtrip(new List<object> { new[] { typeof(IEnumerable<Type[]>), }, })[0])[0]);
                });
        }

        void UsingSerializer(
            Action<DataContractSerializerWrapper<List<object>>> action)
        {
            action(
                new DataContractSerializerWrapper<List<object>>(
                    new DataContractSerializerSettings()
                    .Apply(new TypeDataContractSurrogate())));
        }
    }
}
