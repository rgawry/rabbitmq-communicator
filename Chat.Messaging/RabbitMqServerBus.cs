using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class RabbitMqServerBus : IServerBus, IDisposable
    {
        protected readonly string _exchangeName;
        protected readonly string _requestQueueName;
        protected ConnectionFactory _factory;
        protected IConnection _connection;
        protected Dictionary<string, IModel> _channelsConsume;
        protected IModel _channelProduce;

        public RabbitMqServerBus(string exchangeName, string requestQueueName)
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
        }

        public void AddHandler<TRequest>(Action<TRequest> handler)
        {
            var consumerKey = GetGuid();
            _channelsConsume.Add(consumerKey, _connection.CreateModel());

            var consumer = new EventingBasicConsumer(_channelsConsume[consumerKey]);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var requestMessage = JsonConvert.DeserializeObject<TRequest>(bodyJson);

                handler(requestMessage);
            };
            _channelsConsume[consumerKey].BasicConsume(_requestQueueName, true, consumer);
        }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            var consumerKey = GetGuid();
            _channelsConsume.Add(consumerKey, _connection.CreateModel());

            var consumer = new EventingBasicConsumer(_channelsConsume[consumerKey]);
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
                    _channelProduce.BasicPublish(_exchangeName, responseToQueueName, null, body);
                });
            };
            _channelsConsume[consumerKey].BasicConsume(_requestQueueName, true, consumer);
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

        private string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
