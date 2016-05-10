using RabbitMQ.Client;
using System;

namespace Chat
{
    public sealed class RabbitMqClientBusFactory : IRabbitMqClientBusFactory, IDisposable
    {
        private Configuration _config;
        private IConnection _connection;

        public RabbitMqClientBusFactory(Configuration configuration)
        {
            _config = configuration;
        }

        public RabbitMqClientBus Create(IMessageSerializer messageSerializer)
        {
            _connection = new ConnectionFactory { HostName = _config.HostName, Port = _config.Port }.CreateConnection();
            return new RabbitMqClientBus(_config.ExchangeRequestName, _config.QueueRequestName, _connection, messageSerializer);
        }

        public void Dispose()
        {
            if (_connection != null) _connection.Dispose();
        }
    }
}
