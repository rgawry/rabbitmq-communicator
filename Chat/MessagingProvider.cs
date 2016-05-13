using Castle.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class MessagingProvider : IMessagingProvider, IInitializable, IDisposable
    {
        private CompositeDisposable _thisDisposer = new CompositeDisposable();
        private readonly string _exchangeName;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;
        private EventingBasicConsumer _consumer;
        private EventHandler<BasicDeliverEventArgs> _consumerReceivedHandler;
        private EventHandler<EnvelopeDeliveryEventArgs> _handler;

        public MessagingProvider(string exchangeName, IConnection connection)
        {
            _exchangeName = exchangeName;
            _connection = connection;
        }

        public void Initialize()
        {
            _channelConsume = _connection.CreateModel().DisposeWith(_thisDisposer);
            _channelProduce = _connection.CreateModel().DisposeWith(_thisDisposer);
        }

        public string Create()
        {
            var queueName = _channelProduce.QueueDeclare().QueueName;
            _channelProduce.QueueBind(queueName, _exchangeName, queueName);
            return queueName;
        }

        public void Receive(EventHandler<EnvelopeDeliveryEventArgs> handler)
        {
            _handler = handler;
        }

        public void Send(Envelope envelope)
        {
            var properties = _channelProduce.CreateBasicProperties();
            if (envelope.ReplyTo != null) properties.ReplyTo = envelope.ReplyTo;
            properties.CorrelationId = envelope.CorrelationId;
            properties.Type = envelope.BodyType.ToString();
            _channelProduce.BasicPublish(_exchangeName, envelope.SendTo, properties, envelope.Body);
        }

        public void ListenOn(string streamName)
        {
            _consumerReceivedHandler = (sender, args) =>
            {
                Task.Run(() =>
                {
                    var envelope = new Envelope
                    {
                        Body = args.Body,
                        CorrelationId = args.BasicProperties.CorrelationId,
                        ReplyTo = args.BasicProperties.ReplyTo,
                        BodyType = args.BasicProperties.Type
                    };
                    if (_handler != null) _handler(this, new EnvelopeDeliveryEventArgs(envelope));
                });
            };
            _consumer = new EventingBasicConsumer(_channelConsume);
            _consumer.Received += _consumerReceivedHandler;
            Disposable.Create(() => _consumer.Received -= _consumerReceivedHandler).DisposeWith(_thisDisposer);
            _channelConsume.BasicConsume(streamName, true, _consumer);
        }

        public void Dispose()
        {
            _thisDisposer.Dispose();
        }
    }
}
