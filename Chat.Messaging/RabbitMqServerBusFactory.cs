using RabbitMQ.Client;
using System;

namespace Chat
{
    public sealed class RabbitMqServerBusFactory : IRabbitMqServerBusFactory, IDisposable
    {
        private Configuration _config;
        private IConnection _connection;

        public RabbitMqServerBusFactory(Configuration configuration)
        {
            _config = configuration;
        }

        public RabbitMqServerBus Create(IMessageSerializer messageSerializer)
        {
            _connection = new ConnectionFactory { HostName = _config.HostName, Port = _config.Port }.CreateConnection();
            return new RabbitMqServerBus(_config.ExchangeRequestName, _config.QueueRequestName, _connection, messageSerializer);
        }

        public void Dispose()
        {
            if (_connection != null) _connection.Dispose();
        }
    }
}
