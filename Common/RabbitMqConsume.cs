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
    //?
    public interface IProducer
    {
        void Send();
    }

    public interface IConsumer
    {
        event Action<string> ReceiveHandler;
        void SetRouteKey(string key);
        void Consume();
    }

    public abstract class RabbitMqBase : IDisposable
    {
        protected IConnection _connection;
        protected IModel _channel;

        protected ExchangeDetails _exchangeDetails;
        protected QueueDetails _queueDetails;


        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }

    public class ExchangeDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool AutoDelete { get; set; }
    }

    public class QueueDetails
    {
        public string Name { get; set; }
        public bool AutoDelete { get; set; }
    }

    public class RabbitMqConsumer : RabbitMqBase, IConsumer
    {
        private string _routeKey;
        public event Action<string> ReceiveHandler;

        public RabbitMqConsumer(IConnection connection, ExchangeDetails exchangeDetails, QueueDetails queueDetails)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeDetails.Name, _exchangeDetails.Type, false, _exchangeDetails.AutoDelete, null);
            _channel.QueueDeclare(_queueDetails.Name, false, false, _queueDetails.AutoDelete, null);
            _channel.QueueBind(_queueDetails.Name, _exchangeDetails.Name, "", null);
        }
        
        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ReceivedCallback;
            _channel.BasicConsume(_queueDetails.Name, true, consumer);
        }

        private void ReceivedCallback(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var messageJson = Encoding.UTF8.GetString(body);

            if (ReceiveHandler != null) ReceiveHandler(messageJson);
        }

        public void SetRouteKey(string key)
        {
            _channel.QueueBind(_queueDetails.Name, _exchangeDetails.Name, key, null);
        }
    }
}
