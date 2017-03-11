using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace OhNoPub.MefCacherUnitTest.Runtime.Serialization
{
    /// <summary>
    ///   Me figuring out how to use DataContractSerializer.
    /// </summary>
    [TestClass]
    public class DataContractSerializerUnitTests
    {
        [TestMethod]
        public void SerializeInt()
        {
            var i = 2;
            var serializer = new DataContractSerializer(
                typeof(int),
                new DataContractSerializerSettings());
            using (var buf = new MemoryStream()) {
                using (var writer = XmlWriter.Create(buf))
                {
                    serializer.WriteObject(writer, i);
                }

                // Deserialize.
                buf.Seek(0, SeekOrigin.Begin);
                using (var reader = XmlReader.Create(buf))
                {
                    var result = (int)serializer.ReadObject(reader);
                    Assert.AreEqual(i, result);
                }
            }
        }
    }
}
