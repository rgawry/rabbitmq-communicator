using System;

namespace Chat
{
    public class Envelope
    {
        public string CorrelationId { get; set; }
        public string ReplyTo { get; set; }
        public byte[] Body { get; set; }
        public Type BodyType { get; set; }
    }
}
