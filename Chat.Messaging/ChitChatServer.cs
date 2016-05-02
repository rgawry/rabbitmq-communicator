using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Messaging
{
    public class ChitChatServer : IChitChatServer
    {
        private IClientBus _clientBus;

        public ChitChatServer(IClientBus clientBus)
        {
            _clientBus = clientBus;
        }

        public async Task<bool> TryLogin(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("userName");

            var request = new OpenSessionRequest { UserName = userName, };
            var response = await _clientBus.Request<OpenSessionRequest, OpenSessionResponse>(request);
            return response.IsLogged;
        }
    }
}
