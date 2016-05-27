using Castle.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Chat
{
    public sealed class ServerBus : IServerBus, IInitializable
    {
        private string _requestName;
        private IMessageSerializer _messageSerializer;
        private IMessagingProvider _messagingProvider;
        private ConcurrentDictionary<Type, List<RequestHandler>> _actionRequestsHandlers = new ConcurrentDictionary<Type, List<RequestHandler>>();
        private ConcurrentDictionary<Type, List<RequestHandler>> _funcRequestsHandlers = new ConcurrentDictionary<Type, List<RequestHandler>>();

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

        public void AddHandler<TRequest>(Action<TRequest> handler)
        {
            var requestType = typeof(TRequest);
            if (!_actionRequestsHandlers.ContainsKey(requestType)) _actionRequestsHandlers.TryAdd(requestType, new List<RequestHandler>());
            var requestHandler = RequestHandler.Create(handler);
            _actionRequestsHandlers[requestType].Add(requestHandler);
        }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            var requestType = typeof(TRequest);
            if (!_funcRequestsHandlers.ContainsKey(requestType)) _funcRequestsHandlers.TryAdd(requestType, new List<RequestHandler>());
            var requestHandler = RequestHandler.Create(handler);
            _funcRequestsHandlers[requestType].Add(requestHandler);
        }

        private void DeliveryHandler(object s, EnvelopeDeliveryEventArgs ea)
        {
            var requestEnvelope = ea.Envelope;
            var requestType = Type.GetType(ea.Envelope.BodyType);
            var requestMessage = _messageSerializer.Deserialize(requestEnvelope.Body, requestType);

            var actionRequestHandlers = new List<RequestHandler>();
            if (_actionRequestsHandlers.TryGetValue(requestType, out actionRequestHandlers))
            {
                foreach (var handler in actionRequestHandlers)
                {
                    handler.InvokeAction(requestMessage);
                }
            }

            var funcRequestHandlers = new List<RequestHandler>();
            if (_funcRequestsHandlers.TryGetValue(requestType, out funcRequestHandlers))
            {
                foreach (var handler in funcRequestHandlers)
                {
                    var response = handler.InvokeFunc(requestMessage);

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

        class RequestHandler
        {
            public Action<object> InvokeAction { get; set; }
            public Func<object, object> InvokeFunc { get; set; }

            public static RequestHandler Create<TRequest>(Action<TRequest> handler)
            {
                var result = new RequestHandler();
                result.InvokeAction = arg => handler((TRequest)arg);
                return result;
            }

            public static RequestHandler Create<TRequest, TResponse>(Func<TRequest, TResponse> handler)
            {
                var result = new RequestHandler();
                result.InvokeFunc = arg => handler((TRequest)arg);
                return result;
            }
        }
    }
}
