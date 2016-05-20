using Castle.Core;
using System;
using System.Collections.Concurrent;

namespace Chat
{
    public sealed class ServerBus : IServerBus, IInitializable
    {
        private string _requestName;
        private IMessageSerializer _messageSerializer;
        private IMessagingProvider _messagingProvider;
        private ConcurrentDictionary<Type, Delegate> _requestsHandlers = new ConcurrentDictionary<Type, Delegate>();

        public ServerBus(IMessageSerializer messageSerializer, IMessagingProvider messaggingProvider, string requestName)
        {
            _requestName = requestName;
            _messageSerializer = messageSerializer;
            _messagingProvider = messaggingProvider;
        }

        public void Initialize()
        {
            _messagingProvider.Receive(DeliveryHandler, _requestName);
        }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            var requestType = typeof(TRequest);
            if (_requestsHandlers.ContainsKey(requestType)) throw new ArgumentException("Handler already registered.");
            _requestsHandlers.TryAdd(requestType, handler);
        }

        private void DeliveryHandler(object s, EnvelopeDeliveryEventArgs ea)
        {
            var requestEnvelope = ea.Envelope;
            var requestType = Type.GetType(ea.Envelope.BodyType);
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
