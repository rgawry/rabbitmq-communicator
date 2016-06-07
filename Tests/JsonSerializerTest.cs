using NUnit.Framework;
using System;

namespace Chat
{
    [TestFixture]
    class JsonSerializerTest
    {
        class ClassToSerialize
        {
            public string Item1 { get; set; }
        }

        [Test]
        public void SerializeDeserializeGuid()
        {
            var toSerialize = new ClassToSerialize { Item1 = Guid.NewGuid().ToString() };
            var jsonSerializer = new JsonMessageSerializer();

            var serialized = jsonSerializer.Serialize(toSerialize);
            var deserialized1 = jsonSerializer.Deserialize<ClassToSerialize>(serialized);
            var deserialized2 = jsonSerializer.Deserialize(serialized, typeof(ClassToSerialize)) as ClassToSerialize;

            Assert.AreEqual(toSerialize.Item1, deserialized1.Item1);
            Assert.AreEqual(toSerialize.Item1, deserialized2.Item1);
        }

        [Test]
        public void SerializeDeserializeGuid2()
        {
            var toSerialize = new OpenSessionRequest { Token = Guid.NewGuid().ToString(), UserName = "testName" };
            var jsonSerializer = new JsonMessageSerializer();

            var serialized = jsonSerializer.Serialize(toSerialize);
            var deserialized1 = jsonSerializer.Deserialize<OpenSessionRequest>(serialized);
            var deserialized2 = jsonSerializer.Deserialize(serialized, typeof(OpenSessionRequest)) as OpenSessionRequest;

            Assert.AreEqual(toSerialize.Token, deserialized1.Token);
            Assert.AreEqual(toSerialize.Token, deserialized2.Token);
        }
    }
}
