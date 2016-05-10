﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class RabbitMqServerBus : IServerBus, IDisposable
    {
        private CompositeDisposable _thisDisposer = new CompositeDisposable();
        private readonly string _exchangeName;
        private readonly string _requestQueueName;
        private IMessageSerializer _messageSerializer;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;
        private EventingBasicConsumer _consumerRequest;
        private EventingBasicConsumer _consumerRequestResponse;
        private EventHandler<BasicDeliverEventArgs> _handlerRequest;
        private EventHandler<BasicDeliverEventArgs> _handlerRequestResponse;

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
        }

        public void AddHandler<TRequest>(Action<TRequest> handler)
        {
            _consumerRequest = new EventingBasicConsumer(_channelConsume);
            _handlerRequest = (sender, args) =>
            {
                Task.Run(() =>
                {
                    var requestMessage = _messageSerializer.Deserialize<TRequest>(args.Body);
                    handler(requestMessage);
                });
            };
            _consumerRequest.Received += _handlerRequest;
            _channelConsume.BasicConsume(_requestQueueName, true, _consumerRequest);
        }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            _consumerRequestResponse = new EventingBasicConsumer(_channelConsume);
            _handlerRequestResponse = (sender, args) =>
            {
                Task.Run(() =>
                {
                    var requestMessage = _messageSerializer.Deserialize<TRequest>(args.Body);
                    var responseToQueueName = args.BasicProperties.ReplyTo;

                    var response = handler(requestMessage);

                    var body = _messageSerializer.Serialize(response);
                    var replyProperties = _channelProduce.CreateBasicProperties();
                    var type = typeof(TResponse);
                    replyProperties.Type = type.ToString() + ", " + type.Assembly.FullName;
                    replyProperties.CorrelationId = args.BasicProperties.CorrelationId;
                    _channelProduce.BasicPublish(_exchangeName, responseToQueueName, replyProperties, body);
                });
            };
            _consumerRequestResponse.Received += _handlerRequestResponse;
            _channelConsume.BasicConsume(_requestQueueName, true, _consumerRequestResponse);
        }

        public void Dispose()
        {
            if (_consumerRequest != null) _consumerRequest.Received -= _handlerRequest;
            if (_consumerRequestResponse != null) _consumerRequestResponse.Received -= _handlerRequestResponse;
            _thisDisposer.Dispose();
        }
    }
}
