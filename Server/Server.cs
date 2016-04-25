using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IServer : IDisposable
    {
        void Connect();
        void Bind(string name);
    }

    public class Server : IServer
    {
        private ConnectionFactory _connectionFactory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
        private IConnection _connection;
        private IModel _channel;
        private IList<string> _declaredQueues;

        public void Bind(string name)
        {
            string queueName = string.Empty;
            if (!_declaredQueues.Contains(name))
            {
                queueName = _channel.QueueDeclare().QueueName;
                _declaredQueues.Add(queueName);
            }
            _channel.QueueBind(queue: queueName, exchange: "", routingKey: name);
        }

        public void Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_channel != null) _channel.Dispose();
            if (_connection != null) _channel.Dispose();
        }
    }

    class ServerApp
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "communicator-main",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
        }
    }
}
