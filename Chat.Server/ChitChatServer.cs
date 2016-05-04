using System.Collections.Generic;

namespace Chat
{
    public sealed class ChitChatServer
    {
        private List<string> _users;
        private Dictionary<string, List<string>> _rooms;

        public ChitChatServer()
        {
            _users = new List<string>();
            _rooms = new Dictionary<string, List<string>>();
        }

        public OpenSessionResponse SessionHandler(OpenSessionRequest request)
        {
            var isLogged = false;
            if (!_users.Contains(request.UserName))
            {
                isLogged = true;
                _users.Add(request.UserName);
            }
            return new OpenSessionResponse { IsLogged = isLogged };
        }

        public void SwitchRoomHandler(JoinRoomRequest request)
        {

        }
    }
}
