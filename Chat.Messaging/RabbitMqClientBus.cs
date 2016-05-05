using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class RabbitMqClientBus : IClientBus, IDisposable
    {
        private string _tmpQueue;

        protected readonly string _exchangeName;
        protected readonly string _requestQueueName;
        protected ConnectionFactory _factory;
        protected IConnection _connection;
        protected Dictionary<string, IModel> _channelsConsume;
        protected IModel _channelProduce;

        public RabbitMqClientBus(string exchangeName, string requestQueueName)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
        }

        public void Init()
        {
            _factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channelsConsume = new Dictionary<string, IModel>();
            _channelProduce = _connection.CreateModel();
            _tmpQueue = _channelProduce.QueueDeclare().QueueName;
        }

        public void Request<TRequest>(TRequest request)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, null, body);
        }

        public Task<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            var result = new TaskCompletionSource<TResponse>();
            var responseQueueName = string.Empty;
            var consumerKey = GetConsumerKey();
            _channelsConsume.Add(consumerKey, _connection.CreateModel());

            responseQueueName = _tmpQueue; //_channelsConsume[consumerKey].QueueDeclare().QueueName;
            _channelsConsume[consumerKey].QueueBind(responseQueueName, _exchangeName, responseQueueName);
            var consumer = new EventingBasicConsumer(_channelsConsume[consumerKey]);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var responseMessage = JsonConvert.DeserializeObject<TResponse>(bodyJson);
                result.SetResult(responseMessage);
            };
            _channelsConsume[consumerKey].BasicConsume(responseQueueName, true, consumer);

            var basicProperties = _channelProduce.CreateBasicProperties();
            basicProperties.ReplyTo = responseQueueName;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, basicProperties, body);

            return result.Task;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var consumer in _channelsConsume)
                {
                    consumer.Value.Dispose();
                }
                _channelProduce.Dispose();
                _connection.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private string GetConsumerKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
