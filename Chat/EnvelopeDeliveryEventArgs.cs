using System;

namespace Chat
{
    public class EnvelopeDeliveryEventArgs : EventArgs
    {
        public Envelope Envelope { get; set; }

        public EnvelopeDeliveryEventArgs(Envelope envelope)
        {
            Envelope = envelope;
        }
    }
}
