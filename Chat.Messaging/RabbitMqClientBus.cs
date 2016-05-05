using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class RabbitMqClientBus : Bus, IClientBus
    {
        private string _tmpQueue;

        public RabbitMqClientBus(string exchangeName, string requestQueueName) : base(exchangeName, requestQueueName)
        {
        }

        public void Request<TRequest>(TRequest request)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, null, body);
        }

        public new void Init()
        {
            base.Init();
            _tmpQueue = _channelProduce.QueueDeclare().QueueName;
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
    }
}
