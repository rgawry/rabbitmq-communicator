using System;

namespace Chat
{
    public interface IMessageSerializer
    {
        object Deserialize(byte[] message, Type type);
        T Deserialize<T>(byte[] message);
        byte[] Serialize(object message);
    }
}
