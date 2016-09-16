using System;

namespace Chat
{
    public interface IMessagingProvider
    {
        void Receive(EventHandler<EnvelopeDeliveryEventArgs> handler, string channel, bool multicast);
        void Send(Envelope envelope, string channel, bool multicast);
    }
}
