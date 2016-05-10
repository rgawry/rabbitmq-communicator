using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var clientBus = GetClientBus();
                clientBus.Initialize();
                clientBus.TimeoutValue = 99f;

                Console.WriteLine("Login: ");
                var login = Console.ReadLine();
                var request = new OpenSessionRequest { UserName = login };
                var response = await clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request);
                if (response.IsLogged)
                {
                    Console.WriteLine("Loged in as '" + login + "'");
                }
                else
                {
                    Console.WriteLine("Cant log in as '" + login + "'");
                }
            }).Wait();
        }

        private static IConnection CreateConnection()
        {
            var config = new Configuration();
            var connectionFactory = new ConnectionFactory { HostName = config.HostName, Port = config.Port };
            return connectionFactory.CreateConnection();
        }

        private static RabbitMqClientBus GetClientBus()
        {
            var messageSerializer = new JsonMessageSerializer();
            var exchangeName = "session-exchange";
            var queueName = "session-request";
            var connectionClient = CreateConnection();

            return new RabbitMqClientBus(exchangeName, queueName, connectionClient, messageSerializer);
        }
    }
}
