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
        IModel channel;
        IModel channel2;

        public RabbitMqBus(string exchangeName, string requestQueueName)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
            connection = _factory.CreateConnection();
            channel = connection.CreateModel();
            channel2 = connection.CreateModel();
        }

        public Task<TResult> Request<TRequest, TResult>(TRequest request)
        {
            TaskCompletionSource<TResult> result = new TaskCompletionSource<TResult>();
            var responseQueueName = string.Empty;

            channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            responseQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(responseQueueName, _exchangeName, responseQueueName);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var responseMessage = JsonConvert.DeserializeObject<TResult>(bodyJson);
                result.SetResult(responseMessage);
            };
            channel.BasicConsume(responseQueueName, true, consumer);

            channel2.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            channel2.QueueDeclare(_requestQueueName, false, false, false, null);
            channel2.QueueBind(_requestQueueName, _exchangeName, _requestQueueName);

            var basicProperties = channel2.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>();
            basicProperties.Headers.Add(_responseQueueHeaderKey, responseQueueName);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            channel2.BasicPublish(_exchangeName, _requestQueueName, basicProperties, body);

            return result.Task;
        }

        public void Dispose()
        {
            channel.Dispose();
            channel2.Dispose();
            connection.Dispose();
        }
    }
}
