using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace OhNoPub.MefCacherUnitTest.Serialization
{
    internal class DataContractSerializerWrapper<T>
    {
        DataContractSerializer Serializer { get; }

        public DataContractSerializerWrapper(
            DataContractSerializerSettings settings)
        {
            Serializer = new DataContractSerializer(typeof(T), settings);
        }

        public string Serialize(
            T o)
        {
            var buf = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(
                buf,
                new XmlWriterSettings
                {
                    Indent = true,
                }))
            {
                Serializer.WriteObject(xmlWriter, o);
            }
            var s = $"{buf}";
            Console.WriteLine(s);
            return s;
        }

        public T Deserialize(
            string buf)
        {
            using (var xmlReader = XmlReader.Create(new StringReader(buf)))
                return (T)Serializer.ReadObject(xmlReader);
        }

        public T Roundtrip(T o) => Deserialize(Serialize(o));
    }
}
