using System.Threading.Tasks;

namespace Chat
{
    /// <summary>
    /// Provides data transportation layer for client application.
    /// </summary>
    public interface IClientBus
    {
        /// <summary>
        /// Sends request to ChatServer. Does not expect response.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        void Request<TRequest>(TRequest request);

        /// <summary>
        /// Sends request to ChatServer and returns response.
        /// </summary>
        /// <typeparam name="TRequest">Type of request.</typeparam>
        /// <typeparam name="TResult">Type of data to receive.</typeparam>
        /// <param name="request">Request object.</param>
        /// <returns>Response object.</returns>
        Task<TResult> Request<TRequest, TResult>(TRequest request);
    }
}
