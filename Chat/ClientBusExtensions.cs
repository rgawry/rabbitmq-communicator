using System.Threading.Tasks;

namespace Chat
{
    public static class ClientBusExtensions
    {
        public static RequestContinuation<TRequest> Request<TRequest>(this IClientBus @this, TRequest request)
        {
            return new RequestContinuation<TRequest>(@this, request);
        }

        public class RequestContinuation<TRequest>
        {
            private IClientBus _clientBus;
            private TRequest _request;

            public RequestContinuation(IClientBus clientBus, TRequest request)
            {
                _clientBus = clientBus;
                _request = request;
            }

            public Task<TResponse> Response<TResponse>()
            {
                return _clientBus.Request<TRequest, TResponse>(_request);
            }
        }
    }
}
