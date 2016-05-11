using RabbitMQ.Client;
using System;

namespace Chat
{
    public sealed class RabbitMqServerBusFactory : IRabbitMqServerBusFactory, IDisposable
    {
        private Configuration _config;
        private IConnection _connection;
        private IMessageSerializer _messageSerializer;

        public RabbitMqServerBusFactory(Configuration configuration, IMessageSerializer messageSerializer)
        {
            _config = configuration;
            _messageSerializer = messageSerializer;
        }

        public RabbitMqServerBus Create()
        {
            _connection = new ConnectionFactory { HostName = _config.HostName, Port = _config.Port }.CreateConnection();
            return new RabbitMqServerBus(_config.ExchangeRequestName, _config.QueueRequestName, _connection, _messageSerializer);
        }

        public void Dispose()
        {
            if (_connection != null) _connection.Dispose();
        }
    }
}
