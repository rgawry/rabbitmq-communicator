﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class RabbitMqClientBus : IClientBus, IDisposable
    {
        private string _responseQueueName;
        private readonly string _exchangeName;
        private readonly string _requestQueueName;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;
        private ConcurrentDictionary<string, Response> _taskCollection = new ConcurrentDictionary<string, Response>();

        public RabbitMqClientBus(string exchangeName, string requestQueueName)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
        }

        public void Init()
        {
            _factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channelConsume = _connection.CreateModel();
            _channelProduce = _connection.CreateModel();
            _responseQueueName = _channelProduce.QueueDeclare().QueueName;
            BindToResponseQueue();
        }

        public void Request<TRequest>(TRequest request)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, null, body);
        }

        public Task<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            var correlationId = GetGuid();

            var basicProperties = _channelProduce.CreateBasicProperties();
            basicProperties.ReplyTo = _responseQueueName;
            basicProperties.CorrelationId = correlationId;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, basicProperties, body);

            var responseHandler = Response.Create<TResponse>();

            _taskCollection.TryAdd(correlationId, responseHandler);

            return ((TaskCompletionSource<TResponse>)responseHandler.Tcs).Task;
        }

        private void BindToResponseQueue()
        {
            _channelConsume.QueueBind(_responseQueueName, _exchangeName, _responseQueueName);
            var consumer = new EventingBasicConsumer(_channelConsume);
            consumer.Received += (sender, args) =>
            {
                var responseTypeName = Type.GetType(args.BasicProperties.Type);
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var responseMessage = JsonConvert.DeserializeObject(bodyJson, responseTypeName);
                var key = args.BasicProperties.CorrelationId;
                var responseHandler = default(Response);
                _taskCollection.TryRemove(key, out responseHandler);
                if (responseHandler == null) return;
                responseHandler.OnMessage(responseMessage);
            };
            _channelConsume.BasicConsume(_responseQueueName, true, consumer);
        }

        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public void Dispose()
        {
            _channelConsume.Dispose();
            _channelProduce.Dispose();
            _connection.Dispose();
        }

        class Response
        {
            public object Tcs { get; private set; }
            public Action<object> OnMessage { get; private set; }
            public Action OnTimeout { get; private set; }

            public static Response Create<TResponse>()
            {
                var result = new Response();
                var tcs = new TaskCompletionSource<TResponse>();
                result.OnMessage = it => tcs.SetResult((TResponse)it);
                result.OnTimeout = () => tcs.SetException(new Exception());
                result.Tcs = tcs;
                return result;
            }
        }
    }
}
