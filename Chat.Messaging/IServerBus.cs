using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <param name="handler">Handler that will be invoked when received message.</param>
        void AddHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler);
    }
}
