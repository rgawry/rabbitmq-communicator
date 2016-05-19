using Newtonsoft.Json;
using System;
using System.Text;

namespace Chat
{
    public sealed class JsonMessageSerializer : IMessageSerializer
    {
        public object Deserialize(byte[] message, Type type)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message), type);
        }

        public T Deserialize<T>(byte[] message)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message));
        }

        public byte[] Serialize(object message)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        }
    }
}
