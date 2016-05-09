using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class RabbitMqClientBus : IClientBus, IDisposable
    {
        private const float DEFAULT_TIMEOUT_VALUE = 5;

        private CompositeDisposable _thisDisposer = new CompositeDisposable();
        private CancellationTokenSource _cancelationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancelationToken;
        private readonly string _exchangeName;
        private readonly string _requestQueueName;
        private string _responseQueueName;
        private IMessageSerializer _messageSerializer;
        private IConnection _connection;
        private IModel _channelConsume;
        private IModel _channelProduce;
        private EventingBasicConsumer _consumer;
        private EventHandler<BasicDeliverEventArgs> _handler;
        private ConcurrentDictionary<string, TaskCompletionSourceWrapper> _taskCollection = new ConcurrentDictionary<string, TaskCompletionSourceWrapper>();

        public float TimeoutValue { get; set; }

        public RabbitMqClientBus(string exchangeName, string requestQueueName, IConnection connection, IMessageSerializer messageSerializer)
        {
            _exchangeName = exchangeName;
            _requestQueueName = requestQueueName;
            _connection = connection;
            _messageSerializer = messageSerializer;
        }

        public void Initialize()
        {
            TimeoutValue = DEFAULT_TIMEOUT_VALUE;
            _channelConsume = _connection.CreateModel().DisposeWith(_thisDisposer);
            _channelProduce = _connection.CreateModel().DisposeWith(_thisDisposer);
            _responseQueueName = _channelProduce.QueueDeclare().QueueName;
            _cancelationToken = _cancelationTokenSource.Token;
            BindToResponseQueue();
            Task.Factory.StartNew(ScanForTimeout, _cancelationToken);
        }

        public void Request<TRequest>(TRequest request)
        {
            var body = _messageSerializer.Serialize(request);
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, null, body);
        }

        public Task<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            var correlationId = Guid.NewGuid().ToString();

            var basicProperties = _channelProduce.CreateBasicProperties();
            basicProperties.ReplyTo = _responseQueueName;
            basicProperties.CorrelationId = correlationId;

            var body = _messageSerializer.Serialize(request);
            _channelProduce.BasicPublish(_exchangeName, _requestQueueName, basicProperties, body);

            var responseHandler = TaskCompletionSourceWrapper.Create<TResponse>();
            _taskCollection.TryAdd(correlationId, responseHandler);

            return ((TaskCompletionSource<TResponse>)responseHandler.Tcs).Task;
        }

        private void BindToResponseQueue()
        {
            _channelConsume.QueueBind(_responseQueueName, _exchangeName, _responseQueueName);
            _consumer = new EventingBasicConsumer(_channelConsume);
            _handler = (sender, args) =>
            {
                var responseType = Type.GetType(args.BasicProperties.Type);
                var responseMessage = _messageSerializer.Deserialize(args.Body, responseType);
                var correlationId = args.BasicProperties.CorrelationId;
                var responseHandler = default(TaskCompletionSourceWrapper);

                _taskCollection.TryRemove(correlationId, out responseHandler);

                if (responseHandler == null) return;
                responseHandler.OnMessage(responseMessage);
            };
            _consumer.Received += _handler;
            _channelConsume.BasicConsume(_responseQueueName, true, _consumer);
        }

        private async Task ScanForTimeout()
        {
            while (true)
            {
                var now = DateTime.Now;
                foreach (var task in _taskCollection)
                {
                    if (!(task.Value.Created.AddSeconds(TimeoutValue) < now)) continue;

                    var timeoutedTask = default(TaskCompletionSourceWrapper);
                    if (!_taskCollection.TryRemove(task.Key, out timeoutedTask)) continue;

                    timeoutedTask.OnTimeout();
                }
                if (_cancelationToken.IsCancellationRequested) break;
                await Task.Delay(100);
            }
        }

        public void Dispose()
        {
            _cancelationTokenSource.Cancel();
            _cancelationTokenSource.Dispose();
            if (_consumer != null) _consumer.Received -= _handler;
            _thisDisposer.Dispose();
        }

        class TaskCompletionSourceWrapper
        {
            public object Tcs { get; private set; }
            public Action<object> OnMessage { get; private set; }
            public Action OnTimeout { get; private set; }
            public DateTime Created { get; private set; }

            public static TaskCompletionSourceWrapper Create<TResponse>()
            {
                var result = new TaskCompletionSourceWrapper();
                result.Created = DateTime.Now;
                var tcs = new TaskCompletionSource<TResponse>();
                result.OnMessage = res => tcs.TrySetResult((TResponse)res);
                result.OnTimeout = () => tcs.TrySetException(new TimeoutException());
                result.Tcs = tcs;
                return result;
            }
        }
    }
}
