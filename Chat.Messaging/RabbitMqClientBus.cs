using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Chat
{
    public class RabbitMqClientBus : Bus, IClientBus
    {
        public RabbitMqClientBus(string exchangeName, string requestQueueName) : base(exchangeName, requestQueueName) { }

        public async Task Request<TRequest>(TRequest request)
        {
            await Task.Run(() =>
                {
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
                    _channelProduce.BasicPublish(_exchangeName, _requestQueueName, null, body);
                });
        }

        public Task<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            var result = new TaskCompletionSource<TResponse>();
            var responseQueueName = string.Empty;
            var consumerKey = "req-res";
            _channelsConsume.Add(consumerKey, _connection.CreateModel());

            responseQueueName = _channelsConsume[consumerKey].QueueDeclare().QueueName;
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
