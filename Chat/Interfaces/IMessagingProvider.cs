using System;

namespace Chat
{
    public interface IMessagingProvider
    {
        void Receive(EventHandler<EnvelopeDeliveryEventArgs> handler, string requestName);
        void Send(Envelope envelope);
    }
}
