using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public sealed class ChitChatServer
    {
        private List<string> _users;

        public ChitChatServer()
        {
            _users = new List<string>();
        }

        public OpenSessionResponse Handler(OpenSessionRequest request)
        {
            var isLogged = false;
            if (!_users.Contains(request.UserName))
            {
                isLogged = true;
                _users.Add(request.UserName);
            }

            return new OpenSessionResponse { IsLogged = isLogged };
        }
    }
}
