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
    public class RabbitMqClientBus : IClientBus, IDisposable
    {
        private readonly string _exchangeName;
        private readonly string _requestQueueName;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;

        public RabbitMqClientBus(string exchangeName, string requestQueueName)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
        }

        public void Init()
        {
            _factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channelConsume = _connection.CreateModel();
            _channelProduce = _connection.CreateModel();
        }

        public Task<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            var result = new TaskCompletionSource<TResponse>();
            var responseQueueName = string.Empty;

            responseQueueName = _channelConsume.QueueDeclare().QueueName;
            _channelConsume.QueueBind(responseQueueName, _exchangeName, responseQueueName);
            var consumer = new EventingBasicConsumer(_channelConsume);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var responseMessage = JsonConvert.DeserializeObject<TResponse>(bodyJson);
                result.SetResult(responseMessage);
            };
            _channelConsume.BasicConsume(responseQueueName, true, consumer);

            var basicProperties = _channelProduce.CreateBasicProperties();
            basicProperties.ReplyTo = responseQueueName;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, basicProperties, body);

            return result.Task;
        }

        public void Dispose()
        {
            _channelConsume.Dispose();
            _channelProduce.Dispose();
            _connection.Dispose();
        }
    }
}
