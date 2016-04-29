using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IBus : IDisposable
    {
        Task<TResult> Request<TRequest, TResult>(TRequest request);
    }

    public class RabbitMqBus : IBus
    {
        private readonly string _exchangeName;
        private readonly string _requestQueueName;
        private static readonly string _responseQueueHeaderKey = "response-queue";
        private ConnectionFactory _factory = new ConnectionFactory() { HostName = "10.48.13.111", Port = 5672 };

        IConnection connection;
        IModel channelConsume;
        IModel channelProduce;

        public RabbitMqBus(string exchangeName, string requestQueueName)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
            connection = _factory.CreateConnection();
            channelConsume = connection.CreateModel();
            channelProduce = connection.CreateModel();
        }
        
        public Task<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            TaskCompletionSource<TResponse> result = new TaskCompletionSource<TResponse>();
            var responseQueueName = string.Empty;

            channelConsume.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            responseQueueName = channelConsume.QueueDeclare().QueueName;
            channelConsume.QueueBind(responseQueueName, _exchangeName, responseQueueName);
            var consumer = new EventingBasicConsumer(channelConsume);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var responseMessage = JsonConvert.DeserializeObject<TResponse>(bodyJson);
                result.SetResult(responseMessage);
            };
            channelConsume.BasicConsume(responseQueueName, true, consumer);

            channelProduce.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            channelProduce.QueueDeclare(_requestQueueName, false, false, false, null);
            channelProduce.QueueBind(_requestQueueName, _exchangeName, _requestQueueName);
            var basicProperties = channelProduce.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>();
            basicProperties.Headers.Add(_responseQueueHeaderKey, responseQueueName);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            channelProduce.BasicPublish(_exchangeName, _requestQueueName, basicProperties, body);

            return result.Task;
        }

        public void Dispose()
        {
            channelConsume.Dispose();
            channelProduce.Dispose();
            connection.Dispose();
        }
    }
}
