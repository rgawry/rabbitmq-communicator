using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class RabbitMqServerBus : IServerBus, IDisposable
    {
        private CompositeDisposable _thisDisposer = new CompositeDisposable();
        private readonly string _exchangeName;
        private readonly string _requestQueueName;
        private IMessageSerializer _messageSerializer;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;
        private EventingBasicConsumer _consumer;
        private EventHandler<BasicDeliverEventArgs> _consumerReceivedHandler;
        private ConcurrentDictionary<Type, Delegate> _requestsHandlers = new ConcurrentDictionary<Type, Delegate>();

        public RabbitMqServerBus(string exchangeName, string requestQueueName, IConnection connection, IMessageSerializer messageSerializer)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
            _connection = connection;
            _messageSerializer = messageSerializer;
        }

        public void Initialize()
        {
            _channelConsume = _connection.CreateModel().DisposeWith(_thisDisposer);
            _channelProduce = _connection.CreateModel().DisposeWith(_thisDisposer);
            ListenOnRequestQueue();
        }

        public void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            _requestsHandlers.TryAdd(typeof(TRequest), handler);
        }

        private void ListenOnRequestQueue()
        {
            _consumerReceivedHandler = (sender, args) =>
            {
                Task.Run(() =>
                {
                    var requestType = Type.GetType(args.BasicProperties.Type);
                    var requestMessage = _messageSerializer.Deserialize(args.Body, requestType);

                    if (!_requestsHandlers.ContainsKey(requestType)) return;

                    var handler = default(Delegate);
                    _requestsHandlers.TryGetValue(requestType, out handler);

                    var response = handler.DynamicInvoke(requestMessage);

                    var responseToQueueName = args.BasicProperties.ReplyTo;
                    var responseMessageBody = _messageSerializer.Serialize(response);
                    var responseProperties = _channelProduce.CreateBasicProperties();
                    responseProperties.CorrelationId = args.BasicProperties.CorrelationId;
                    _channelProduce.BasicPublish(_exchangeName, responseToQueueName, responseProperties, responseMessageBody);
                });
            };
            _consumer = new EventingBasicConsumer(_channelConsume);
            _consumer.Received += _consumerReceivedHandler;
            _channelConsume.BasicConsume(_requestQueueName, true, _consumer);
        }

        public void Dispose()
        {
            if (_consumer != null) _consumer.Received -= _consumerReceivedHandler;
            _thisDisposer.Dispose();
        }
    }
}
