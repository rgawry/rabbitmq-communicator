using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Chat
{
    public abstract class Bus : IDisposable
    {
        protected readonly string _exchangeName;
        protected readonly string _requestQueueName;
        protected ConnectionFactory _factory;
        protected IConnection _connection;
        protected Dictionary<string, IModel> _channelsConsume;
        protected IModel _channelProduce;

        public Bus(string exchangeName, string requestQueueName)
        {
            if (string.IsNullOrWhiteSpace(exchangeName) || string.IsNullOrWhiteSpace(requestQueueName)) throw new ArgumentException();

            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
        }

        public void Init()
        {
            _factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channelsConsume = new Dictionary<string, IModel>();
            _channelProduce = _connection.CreateModel();
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

        protected string GetConsumerKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
