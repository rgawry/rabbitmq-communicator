using Castle.Core;
using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class ClientBus : IClientBus, IDisposable, IInitializable
    {
        private const float DEFAULT_TIMEOUT_VALUE = 5;

        private CompositeDisposable _thisDisposer = new CompositeDisposable();
        private string _requestName;
        private string _responseName;
        private IMessageSerializer _messageSerializer;
        private IMessagingProvider _messagingProvider;
        private CancellationTokenSource _cancelationTokenSource;
        private CancellationToken _cancelationToken;
        private ConcurrentDictionary<string, ResponseHandler> _taskCollection = new ConcurrentDictionary<string, ResponseHandler>();

        public float TimeoutValue { get; set; }

        public ClientBus(IMessageSerializer messageSerializer, IMessagingProvider messagingProvider, string requestName)
        {
            _messageSerializer = messageSerializer;
            _messagingProvider = messagingProvider;
            _requestName = requestName;
        }

        public void Initialize()
        {
            TimeoutValue = DEFAULT_TIMEOUT_VALUE;
            _responseName = Guid.NewGuid().ToString();
            _messagingProvider.Receive(DeliveryHandler, _responseName);
            Disposable.Create(() => _cancelationTokenSource.Cancel()).DisposeWith(_thisDisposer);
            _cancelationTokenSource = new CancellationTokenSource().DisposeWith(_thisDisposer);
            _cancelationToken = _cancelationTokenSource.Token;
            Task.Factory.StartNew(ScanForTimeout, _cancelationToken);
        }

        public Task<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            var requestType = typeof(TRequest);
            var correlationId = Guid.NewGuid().ToString();
            var responseHandler = ResponseHandler.Create<TResponse>();
            _taskCollection.TryAdd(correlationId, responseHandler);
            var requestMessageBody = _messageSerializer.Serialize(request);

            var requestEnvelope = new Envelope
            {
                CorrelationId = correlationId,
                BodyType = requestType.FullName + ", " + requestType.Assembly.FullName,
                SendTo = _requestName,
                ReplyTo = _responseName,
                Body = requestMessageBody
            };
            _messagingProvider.Send(requestEnvelope);

            return ((TaskCompletionSource<TResponse>)responseHandler.Tcs).Task;
        }

        private void DeliveryHandler(object sender, EnvelopeDeliveryEventArgs args)
        {
            var responseEnvelope = args.Envelope;
            var correlationId = responseEnvelope.CorrelationId;
            var responseHandler = default(ResponseHandler);

            if (!_taskCollection.TryRemove(correlationId, out responseHandler)) return;

            var responseMessage = _messageSerializer.Deserialize(responseEnvelope.Body, responseHandler.ResponseType);
            responseHandler.OnMessage(responseMessage);
        }

        private async Task ScanForTimeout()
        {
            while (true)
            {
                var now = DateTime.Now;
                foreach (var task in _taskCollection)
                {
                    if (!(task.Value.Created.AddSeconds(TimeoutValue) < now)) continue;

                    var timeoutedTask = default(ResponseHandler);
                    if (!_taskCollection.TryRemove(task.Key, out timeoutedTask)) continue;

                    timeoutedTask.OnTimeout();
                }
                if (_cancelationToken.IsCancellationRequested) break;
                await Task.Delay(100);
            }
        }

        public void Dispose()
        {
            _thisDisposer.Dispose();
        }

        class ResponseHandler
        {
            public object Tcs { get; private set; }
            public Action<object> OnMessage { get; private set; }
            public Action OnTimeout { get; private set; }
            public DateTime Created { get; private set; }
            public Type ResponseType { get; set; }

            public static ResponseHandler Create<TResponse>()
            {
                var result = new ResponseHandler();
                result.Created = DateTime.Now;
                result.ResponseType = typeof(TResponse);
                var tcs = new TaskCompletionSource<TResponse>();
                result.OnMessage = res => tcs.TrySetResult((TResponse)res);
                result.OnTimeout = () => tcs.TrySetException(new TimeoutException());
                result.Tcs = tcs;
                return result;
            }
        }
    }
}
