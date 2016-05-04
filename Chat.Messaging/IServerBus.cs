using System;
using System.Threading.Tasks;

namespace Chat
{
    /// <summary>
    /// Provides data transportation layer for server application.
    /// </summary>
    public interface IServerBus
    {
        /// <summary>
        /// Registers listener.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="handler">Handler that will be invoked when received message.</param>
        /// <returns></returns>
        Task AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler);

        /// <summary>
        /// Registers listener.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="handler">Handler that will be invoked when received message.</param>
        /// <returns></returns>
        Task AddHandler<TRequest>(Action<TRequest> handler);
    }
}
