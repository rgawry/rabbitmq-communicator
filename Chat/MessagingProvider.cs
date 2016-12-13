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
        private CompositeDisposable _instanceDisposer = new CompositeDisposable();
        private readonly string _exchangeName;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;
        private EventingBasicConsumer _consumer;
        private EventHandler<BasicDeliverEventArgs> _consumerReceivedHandler;

        public MessagingProvider(string exchangeName, IConnection connection)
        {
            _exchangeName = exchangeName;
            _connection = connection;
        }

        public void Initialize()
        {
            _channelConsume = _connection.CreateModel().DisposeWith(_instanceDisposer);
            _channelProduce = _connection.CreateModel().DisposeWith(_instanceDisposer);
        }

        public void Receive(EventHandler<EnvelopeDeliveryEventArgs> handler, string channel)
        {
            if (handler == null || string.IsNullOrWhiteSpace(channel)) throw new ArgumentException();

            _channelConsume.QueueDeclare(channel, false, false, true, null);
            _channelConsume.QueueBind(channel, _exchangeName, channel);
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
                    handler(this, new EnvelopeDeliveryEventArgs(envelope));
                });
            };
            _consumer = new EventingBasicConsumer(_channelConsume);
            _consumer.Received += _consumerReceivedHandler;
            Disposable.Create(() => _consumer.Received -= _consumerReceivedHandler).DisposeWith(_instanceDisposer);
            _channelConsume.BasicConsume(channel, true, _consumer);
        }

        public void Send(Envelope envelope)
        {
            var properties = _channelProduce.CreateBasicProperties();
            if (envelope.ReplyTo != null) properties.ReplyTo = envelope.ReplyTo;
            if (envelope.CorrelationId != null) properties.CorrelationId = envelope.CorrelationId;
            properties.Type = envelope.BodyType.ToString();
            _channelProduce.BasicPublish(_exchangeName, envelope.SendTo, properties, envelope.Body);
        }

        public void Dispose()
        {
            _instanceDisposer.Dispose();
        }
    }
}
