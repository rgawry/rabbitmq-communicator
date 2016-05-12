using Castle.Core;
using System.Collections.Generic;

namespace Chat
{
    public sealed class ChatServer : IInitializable
    {
        private List<string> _users = new List<string>();
        private Dictionary<string, List<string>> _usersInRooms = new Dictionary<string, List<string>>();

        public Dictionary<string, List<string>> UsersInRooms { get { return _usersInRooms; } }
        public string DefaultRoomName { get { return "default"; } }

        public void Initialize()
        {
            _usersInRooms.Add(DefaultRoomName, new List<string>());
        }

        public OpenSessionResponse SessionHandler(OpenSessionRequest request)
        {
            var isLogged = false;
            if (!_users.Contains(request.UserName))
            {
                isLogged = true;
                _users.Add(request.UserName);
                _usersInRooms[DefaultRoomName].Add(request.UserName);
            }
            return new OpenSessionResponse { IsLogged = isLogged };
        }

        public void SwitchRoomHandler(JoinRoomRequest request)
        {
            RemoveUserFromDefaultRoom(request.Token);
            if (!_usersInRooms.ContainsKey(request.RoomName)) _usersInRooms.Add(request.RoomName, new List<string>());
            _usersInRooms[request.RoomName].Add(request.Token);
        }

        private void RemoveUserFromDefaultRoom(string token)
        {
            var defaultRoomUsers = _usersInRooms[DefaultRoomName];
            if (defaultRoomUsers.Contains(token)) defaultRoomUsers.RemoveAt(defaultRoomUsers.IndexOf(token));
        }
    }
}
