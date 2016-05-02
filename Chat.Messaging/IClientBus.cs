using System.Threading.Tasks;

namespace Chat
{
    /// <summary>
    /// Provides data transportation layer for client application.
    /// </summary>
    public interface IClientBus
    {
        /// <summary>
        /// Sends request to Server and returns response.
        /// </summary>
        /// <typeparam name="TRequest">Type of data to be send.</typeparam>
        /// <typeparam name="TResult">Type of data to receive.</typeparam>
        /// <param name="request">Request data to be send.</param>
        /// <returns>Response</returns>
        Task<TResult> Request<TRequest, TResult>(TRequest request);
    }
}
