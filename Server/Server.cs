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
    public interface IServer
    {
        void ListenForNewSession();
    }

    public class Server : IServer
    {
        private List<string> _users;

        string _exchangeName = "session-exchange";

        public void ListenForNewSession()
        {
            var factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            channel.QueueDeclare("session-request", false, false, false, null);
            channel.QueueBind("session-request", _exchangeName, "session-request");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var responseMessage = JsonConvert.DeserializeObject<OpenSessionRequest>(bodyJson);
                var responseToQueueName = (byte[])args.BasicProperties.Headers["response-queue"];

                var response = new OpenSessionResponse { IsLogged = true };
                var JsonResponse = JsonConvert.SerializeObject(response);
                var body = Encoding.UTF8.GetBytes(JsonResponse);
                channel.BasicPublish(_exchangeName, Encoding.UTF8.GetString(responseToQueueName), null, body);
            };
            channel.BasicConsume("session-request", true, consumer);
        }
    }
}
