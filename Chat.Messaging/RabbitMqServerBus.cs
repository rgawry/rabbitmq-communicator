using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Messaging
{
    public class RabbitMqServerBus : IServerBus, IDisposable
    {
        private string _exchangeName;
        private string _requestQueueName;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;

        public RabbitMqServerBus(string exchangeName, string requestQueueName)
        {
            _factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
        }

        public void Init()
        {
            _connection = _factory.CreateConnection();
            _channelConsume = _connection.CreateModel();
            _channelProduce = _connection.CreateModel();
        }

        public void AddHandler(Func<OpenSessionRequest, OpenSessionResponse> handler)
        {
            var consumer = new EventingBasicConsumer(_channelProduce);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var requestMessage = JsonConvert.DeserializeObject<OpenSessionRequest>(bodyJson);
                var responseToQueueName = args.BasicProperties.ReplyTo;

                var response = handler(requestMessage);

                var jsonResponse = JsonConvert.SerializeObject(response);
                var body = Encoding.UTF8.GetBytes(jsonResponse);
                _channelProduce.BasicPublish(_exchangeName, responseToQueueName, null, body);
            };
            _channelConsume.BasicConsume(_requestQueueName, true, consumer);
        }

        public void Dispose()
        {
            _channelConsume.Dispose();
            _channelProduce.Dispose();
            _connection.Dispose();
        }
    }
}
