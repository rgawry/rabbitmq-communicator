using RabbitMQ.Client;

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverBus = GetServerBus();
            serverBus.Initialize();
            var chatServer = new ChitChatServer();
            chatServer.Initialize();

            serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(chatServer.SessionHandler);
        }

        private static IConnection CreateConnection()
        {
            var config = new Configuration();
            var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
            return connectionFactory.CreateConnection();
        }

        private static RabbitMqServerBus GetServerBus()
        {
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionServer = CreateConnection();

            return new RabbitMqServerBus(exchangeName, queueName, connectionServer, messageSerializer);
        }
    }
}
