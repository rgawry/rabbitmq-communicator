using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Provides data transportation layer for server application.
    /// </summary>
    public interface IServerBus
    {
        /// <summary>
        /// Registers listener.
        /// </summary>
        /// <param name="handler">Handler that will be invoekd when received message.</param>
        void AddHandler(Func<OpenSessionRequest, OpenSessionResponse> handler);
    }
}
