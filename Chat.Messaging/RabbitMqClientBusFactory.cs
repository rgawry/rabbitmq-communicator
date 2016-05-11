using RabbitMQ.Client;
using System;

namespace Chat
{
    public sealed class RabbitMqClientBusFactory : IRabbitMqClientBusFactory, IDisposable
    {
        private Configuration _config;
        private IConnection _connection;
        private IMessageSerializer _messageSerializer;

        public RabbitMqClientBusFactory(Configuration configuration, IMessageSerializer messageSerializer)
        {
            _config = configuration;
            _messageSerializer = messageSerializer;
        }

        public RabbitMqClientBus Create()
        {
            _connection = new ConnectionFactory { HostName = _config.HostName, Port = _config.Port }.CreateConnection();
            return new RabbitMqClientBus(_config.ExchangeRequestName, _config.QueueRequestName, _connection, _messageSerializer);
        }

        public void Dispose()
        {
            if (_connection != null) _connection.Dispose();
        }
    }
}
