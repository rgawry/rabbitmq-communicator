using System;

namespace Chat
{
    public interface IMessagingProvider
    {
        string Create();
        void Receive(EventHandler<EnvelopeDeliveryEventArgs> handler);
        void Send(Envelope envelope);
        void ListenOn(string streamName);
    }
}
