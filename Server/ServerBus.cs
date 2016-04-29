using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerBus : IServerBus, IDisposable
    {
        private string _exchangeName = "session-exchange";
        ConnectionFactory factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
        IConnection connection;
        IModel channel;

        public ServerBus()
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void AddHandler(Func<OpenSessionRequest, OpenSessionResponse> handler)
        {
            channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            channel.QueueDeclare("session-request", false, false, false, null);
            channel.QueueBind("session-request", _exchangeName, "session-request");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var requestMessage = JsonConvert.DeserializeObject<OpenSessionRequest>(bodyJson);
                var responseToQueueName = (byte[])args.BasicProperties.Headers["response-queue"];

                var response = handler(requestMessage);
                
                var jsonResponse = JsonConvert.SerializeObject(response);
                var body = Encoding.UTF8.GetBytes(jsonResponse);
                channel.BasicPublish(_exchangeName, Encoding.UTF8.GetString(responseToQueueName), null, body);
            };
            channel.BasicConsume("session-request", true, consumer);
        }

        public void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }
    }
}
