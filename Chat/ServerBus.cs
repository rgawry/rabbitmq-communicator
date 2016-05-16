using Castle.Core;
using System;
using System.Collections.Concurrent;

namespace Chat
{
    public sealed class ServerBus : IServerBus, IInitializable
    {
        private IMessageSerializer _messageSerializer;
        private IMessagingProvider _messagingProvider;
        private ConcurrentDictionary<Type, Delegate> _requestsHandlers = new ConcurrentDictionary<Type, Delegate>();
        private string _requestStream;

        public ServerBus(IMessageSerializer messageSerializer, IMessagingProvider messaggingProvider, string requestStream)
        {
            _messageSerializer = messageSerializer;
            _messagingProvider = messaggingProvider;
            _requestStream = requestStream;
        }

        public void Initialize()
        {
            _messagingProvider.Receive(DeliveryHandler);
            _messagingProvider.ListenOn(_requestStream);
        }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            var requestType = typeof(TRequest);
            if (_requestsHandlers.ContainsKey(requestType)) throw new ArgumentException("Handler already registered.");
            _requestsHandlers.TryAdd(requestType, handler);
        }

        private void DeliveryHandler(object s, EnvelopeDeliveryEventArgs args)
        {
            var requestEnvelope = args.Envelope;
            var requestType = Type.GetType(args.Envelope.BodyType);
            var requestMessage = _messageSerializer.Deserialize(requestEnvelope.Body, requestType);

            var requestHandler = default(Delegate);
            if (!_requestsHandlers.TryGetValue(requestType, out requestHandler)) return;

            var response = requestHandler.DynamicInvoke(requestMessage);

            var responseMessageBody = _messageSerializer.Serialize(response);
            var responseEnvelope = new Envelope
            {
                Body = responseMessageBody,
                CorrelationId = requestEnvelope.CorrelationId,
                SendTo = requestEnvelope.ReplyTo,
                BodyType = requestEnvelope.BodyType
            };
            _messagingProvider.Send(responseEnvelope);
        }
    }
}
