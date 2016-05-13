using Castle.Core;
using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;

namespace Chat
{
    public sealed class ServerBus : IServerBus, IDisposable, IInitializable
    {
        private IMessageSerializer _messageSerializer;
        private IMessagingProvider _messagingProvider;
        private ConcurrentDictionary<Type, Delegate> _requestsHandlers = new ConcurrentDictionary<Type, Delegate>();
        private CompositeDisposable _thisDisposer = new CompositeDisposable();

        public ServerBus(IMessageSerializer messageSerializer, IMessagingProvider messaggingProvider)
        {
            _messageSerializer = messageSerializer;
            _messagingProvider = messaggingProvider;
        }

        public void Initialize()
        {
            _messagingProvider.Receive(DeliveryHandler);
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
            var requestMessage = _messageSerializer.Deserialize(requestEnvelope.Body, args.Envelope.BodyType);

            var requestHandler = default(Delegate);
            if (!_requestsHandlers.TryGetValue(requestEnvelope.BodyType, out requestHandler)) return;

            var response = requestHandler.DynamicInvoke(requestMessage);

            var responseMessageBody = _messageSerializer.Serialize(response);
            var responseEnvelope = new Envelope
            {
                Body = responseMessageBody,
                CorrelationId = requestEnvelope.CorrelationId,
                ReplyTo = requestEnvelope.ReplyTo,
                BodyType = requestEnvelope.BodyType
            };
            _messagingProvider.Send(responseEnvelope);
        }

        public void Dispose()
        {
            _thisDisposer.Dispose();
        }
    }
}
