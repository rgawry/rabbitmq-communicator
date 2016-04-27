using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IClient
    {

    }

    public class Client : IClient
    {
        public void RequestNewSession(string login, string room)
        {
            var factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("session-exange", ExchangeType.Direct, false, true, null);
                channel.QueueDeclare("session-request", false, false, true, null);
                channel.QueueBind("session-request", "session-exchange", "session-request");
                var request = new OpenSessionRequest() { Login = login, Room = room };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
                channel.BasicPublish("session-exchange", "session-request", null, body);

                channel.ExchangeDeclare("session-exange", ExchangeType.Direct, false, true, null);
                channel.QueueDeclare("session-response", false, false, true, null);
                channel.QueueBind("session-response", "session-exchange", "session-response");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += NewSession;
                channel.BasicConsume(queue: "session-response",
                                 noAck: true,
                                 consumer: consumer);
            }
        }

        private void NewSession(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var messageJson = Encoding.UTF8.GetString(body);
            var response = JsonConvert.DeserializeObject<OpenSessionResponse>(messageJson);
        }
    }
}
