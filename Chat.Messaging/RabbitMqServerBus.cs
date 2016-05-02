﻿using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Chat
{
    public class RabbitMqServerBus : Bus, IServerBus
    {
        public RabbitMqServerBus(string exchangeName, string requestQueueName) : base(exchangeName, requestQueueName) { }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            var consumer = new EventingBasicConsumer(_channelConsume);
            consumer.Received += (sender, args) =>
            {
                var bodyJson = Encoding.UTF8.GetString(args.Body);
                var requestMessage = JsonConvert.DeserializeObject<TRequest>(bodyJson);
                var responseToQueueName = args.BasicProperties.ReplyTo;

                var response = handler(requestMessage);

                var jsonResponse = JsonConvert.SerializeObject(response);
                var body = Encoding.UTF8.GetBytes(jsonResponse);
                _channelProduce.BasicPublish(_exchangeName, responseToQueueName, null, body);
            };
            _channelConsume.BasicConsume(_requestQueueName, true, consumer);
        }
    }
}
