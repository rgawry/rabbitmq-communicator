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

    }

    public class Server : IServer
    {
        private List<string> _rooms;
        private List<string> _users;

        private IConsumer _consumer;

        public Server(IConsumer consumer)
        {
            _consumer = consumer;
        }

        public void ListenForSessionRequest()
        {
            _consumer.Consume();
        }

        private void CheckRequest(OpenSessionRequest request)
        {
            var factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("session-exange", ExchangeType.Direct, false, true, null);
                channel.QueueDeclare("session-response", false, false, true, null);
                channel.QueueBind("session-response", "session-exchange", "session-response");
                var response = new OpenSessionResponse();
                if (_users.Contains(request.Login))
                {
                    response.IsOpen = false;
                }
                else
                {
                    response.IsOpen = true;
                    _users.Add(request.Login);
                    _rooms.Add(request.Room);
                }
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                channel.BasicPublish("session-exchange", "session-response", null, body);
            }
        }
    }
}
