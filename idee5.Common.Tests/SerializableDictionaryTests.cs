using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Serialization;

namespace idee5.Common.Tests {
    [TestClass]
    public class SerializableDictionaryTests {
        [UnitTest, TestMethod]
        public void CanSerializeDictionary() {
            var dict = new SerializableDictionary<string, string>();

            dict.Add(key: "p1", value: "t1");
            dict.Add(key: "p2", value: "t2");
            var serializer1 = new XmlSerializer(typeof(SerializableDictionary<string, string>));

            var stream = new MemoryStream();
            serializer1.Serialize(stream, dict);
            stream.Position = 0;
            // Call the Deserialize method and cast to the object type.
            Assert.IsInstanceOfType((SerializableDictionary<string, string>)serializer1.Deserialize(stream), typeof(SerializableDictionary<string, string>));
        }
    }
}