using System;

namespace Chat
{
    public interface IMessagingProvider
    {
        void Receive(EventHandler<EnvelopeDeliveryEventArgs> handler);
        void Send(Envelope envelope);
    }
}
