using NUnit.Framework;
using System;

namespace Chat
{
    [TestFixture]
    class JsonSerializerTest
    {
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
