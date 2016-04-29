using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IServerBus
    {
        void AddHandler(Func<OpenSessionRequest, OpenSessionResponse> handler);
    }
}
