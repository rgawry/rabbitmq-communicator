using Chat.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class ChitChatServer
    {
        public OpenSessionResponse Handler(OpenSessionRequest request)
        {
            return new OpenSessionResponse { };
        }
    }
}
