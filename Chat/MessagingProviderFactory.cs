using RabbitMQ.Client;
using System;

namespace Chat
{
    public sealed class MessagingProviderFactory : IMessagingProviderFactory, IDisposable
    {
        private Configuration _config;
        private IConnection _connection;

        public MessagingProviderFactory(Configuration configuration)
        {
            _config = configuration;
        }

        public MessagingProvider Create()
        {
            _connection = new ConnectionFactory { HostName = _config.HostName, Port = _config.Port }.CreateConnection();
            return new MessagingProvider(_config.ExchangeRequestName, _config.QueueRequestName, _connection);
        }

        public void Dispose()
        {
            if (_connection != null) _connection.Dispose();
        }
    }
}
