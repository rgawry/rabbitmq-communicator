﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class RabbitMqClientBus : IClientBus, IDisposable
    {
        private string _responseQueueName;
        protected readonly string _exchangeName;
        protected readonly string _requestQueueName;
        protected ConnectionFactory _factory;
        protected IConnection _connection;
        protected Dictionary<string, IModel> _channelsConsume;
        protected IModel _channelProduce;

        public RabbitMqClientBus(string exchangeName, string requestQueueName)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
        }

        public void Init()
        {
            _factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channelsConsume = new Dictionary<string, IModel>();
            _channelProduce = _connection.CreateModel();
            _responseQueueName = _channelProduce.QueueDeclare().QueueName;
        }

        public void Request<TRequest>(TRequest request)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, null, body);
        }

        public Task<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            var result = new TaskCompletionSource<TResponse>();
            var consumerKey = GetGuid();
            _channelsConsume.Add(consumerKey, _connection.CreateModel());

            _channelsConsume[consumerKey].BasicQos(0, 1, false);
            _channelsConsume[consumerKey].QueueBind(_responseQueueName, _exchangeName, _responseQueueName);
            var consumer = new EventingBasicConsumer(_channelsConsume[consumerKey]);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var responseMessage = JsonConvert.DeserializeObject<TResponse>(bodyJson);
                result.SetResult(responseMessage);
            };
            _channelsConsume[consumerKey].BasicConsume(_responseQueueName, true, consumer);

            var basicProperties = _channelProduce.CreateBasicProperties();
            basicProperties.ReplyTo = _responseQueueName;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, basicProperties, body);

            return result.Task;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var consumer in _channelsConsume)
                {
                    consumer.Value.Dispose();
                }
                _channelProduce.Dispose();
                _connection.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
