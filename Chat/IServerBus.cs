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
        /// Registers new request listener. You can register only one listener for given type of request.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="handler">Handler that will be invoked when received message.</param>
        /// <exception cref="ArgumentException">Throws when handler is already registered.</exception>
        void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler);
    }
}
