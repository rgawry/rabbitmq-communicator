using System;

namespace Chat
{
    /// <summary>
    /// Provides data transportation layer for server application.
    /// </summary>
    public interface IServerBus
    {
        /// <summary>
        /// Registers new request listener.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="handler"></param>
        void AddHandler<TRequest>(Action<TRequest> handler);

        /// <summary>
        /// Registers new request listener.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="handler">Handler that will be invoked when received message.</param>
        void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler);
    }
}
