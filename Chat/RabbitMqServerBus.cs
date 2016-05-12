﻿using Castle.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class RabbitMqServerBus : IServerBus, IDisposable, IInitializable
    {
        private CompositeDisposable _thisDisposer = new CompositeDisposable();
        private readonly string _exchangeName;
        private readonly string _requestQueueName;
        private IMessageSerializer _messageSerializer;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;
        private EventingBasicConsumer _consumer;
        private EventHandler<BasicDeliverEventArgs> _consumerReceivedHandler;
        private ConcurrentDictionary<Type, Delegate> _requestsHandlers = new ConcurrentDictionary<Type, Delegate>();

        public RabbitMqServerBus(string exchangeName, string requestQueueName, IConnection connection, IMessageSerializer messageSerializer)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
            _connection = connection;
            _messageSerializer = messageSerializer;
        }

        public void Initialize()
        {
            _channelConsume = _connection.CreateModel().DisposeWith(_thisDisposer);
            _channelProduce = _connection.CreateModel().DisposeWith(_thisDisposer);
            ListenOnRequestQueue();
        }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            var requestType = typeof(TRequest);
            if (_requestsHandlers.ContainsKey(requestType)) throw new ArgumentException("Handler already registered.");
            _requestsHandlers.TryAdd(requestType, handler);
        }

        private void ListenOnRequestQueue()
        {
            _consumerReceivedHandler = (sender, args) =>
            {
                Task.Run(() =>
                {
                    var requestType = Type.GetType(args.BasicProperties.Type);
                    var requestMessage = _messageSerializer.Deserialize(args.Body, requestType);

                    var requestHandler = default(Delegate);
                    if (!_requestsHandlers.TryGetValue(requestType, out requestHandler)) return;

                    var response = requestHandler.DynamicInvoke(requestMessage);

                    var responseQueueName = args.BasicProperties.ReplyTo;
                    var responseMessageBody = _messageSerializer.Serialize(response);
                    var responseProperties = _channelProduce.CreateBasicProperties();
                    responseProperties.CorrelationId = args.BasicProperties.CorrelationId;
                    _channelProduce.BasicPublish(_exchangeName, responseQueueName, responseProperties, responseMessageBody);
                });
            };
            _consumer = new EventingBasicConsumer(_channelConsume);
            _consumer.Received += _consumerReceivedHandler;
            Disposable.Create(() => _consumer.Received -= _consumerReceivedHandler).DisposeWith(_thisDisposer);
            _channelConsume.BasicConsume(_requestQueueName, true, _consumer);
        }

        public void Dispose()
        {
            _thisDisposer.Dispose();
        }
    }
}
