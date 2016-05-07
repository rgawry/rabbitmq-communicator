using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class RabbitMqServerBus : IServerBus, IDisposable
    {
        private readonly string _exchangeName;
        private readonly string _requestQueueName;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;

        public RabbitMqServerBus(string exchangeName, string requestQueueName, IConnection connection)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
            _connection = connection;
        }

        public void Init()
        {
            _channelConsume = _connection.CreateModel();
            _channelProduce = _connection.CreateModel();
        }

        public void AddHandler<TRequest>(Action<TRequest> handler)
        {
            var consumer = new EventingBasicConsumer(_channelConsume);
            consumer.Received += (sender, args) =>
            {
                Task.Run(() =>
                {
                    var bodyJson = Encoding.UTF8.GetString(args.Body);
                    var requestMessage = JsonConvert.DeserializeObject<TRequest>(bodyJson);

                    handler(requestMessage);
                });
            };
            _channelConsume.BasicConsume(_requestQueueName, true, consumer);
        }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            var consumer = new EventingBasicConsumer(_channelConsume);
            consumer.Received += (sender, args) =>
            {
                Task.Run(() =>
                {
                    var bodyJson = Encoding.UTF8.GetString(args.Body);
                    var requestMessage = JsonConvert.DeserializeObject<TRequest>(bodyJson);
                    var responseToQueueName = args.BasicProperties.ReplyTo;

                    var response = handler(requestMessage);

                    var jsonResponse = JsonConvert.SerializeObject(response);
                    var body = Encoding.UTF8.GetBytes(jsonResponse);
                    var replyProperties = _channelProduce.CreateBasicProperties();
                    replyProperties.Type = typeof(TResponse).ToString();
                    replyProperties.CorrelationId = args.BasicProperties.CorrelationId;
                    _channelProduce.BasicPublish(_exchangeName, responseToQueueName, replyProperties, body);
                });
            };
            _channelConsume.BasicConsume(_requestQueueName, true, consumer);
        }

        public void Dispose()
        {
            _channelConsume.Dispose();
            _channelProduce.Dispose();
            _connection.Dispose();
        }

        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
