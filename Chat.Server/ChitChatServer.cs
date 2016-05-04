using System.Collections.Generic;

namespace Chat
{
    public sealed class ChitChatServer
    {
        private const string DEFAULT_ROOM_NAME = "default";

        private List<string> _users;
        private Dictionary<string, List<string>> _usersToRoomMap;

        public ChitChatServer()
        {

        }

        public void Init()
        {
            _users = new List<string>();
            _usersToRoomMap = new Dictionary<string, List<string>>();
            _usersToRoomMap.Add(DEFAULT_ROOM_NAME, new List<string>());
        }

        public OpenSessionResponse SessionHandler(OpenSessionRequest request)
        {
            var isLogged = false;
            if (!_users.Contains(request.UserName))
            {
                isLogged = true;
                _users.Add(request.UserName);
                _usersToRoomMap[DEFAULT_ROOM_NAME].Add(request.UserName);
            }
            return new OpenSessionResponse { IsLogged = isLogged };
        }

        public void SwitchRoomHandler(JoinRoomRequest request)
        {

        }
    }
}
