using System.Collections.Generic;

namespace Chat
{
    public sealed class ChitChatServer
    {
        private List<string> _users;

        public ChitChatServer()
        {
            _users = new List<string>();
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

        public void RoomHandler(JoinRoomRequest request)
        {

        }
    }
}
