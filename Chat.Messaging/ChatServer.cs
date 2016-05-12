using Castle.Core;
using System.Collections.Generic;

namespace Chat
{
    public sealed class ChatServer : IInitializable
    {
        private IDisplay _display;
        private IServerBus _serverBus;
        private List<string> _users = new List<string>();
        private Dictionary<string, List<string>> _usersInRooms = new Dictionary<string, List<string>>();

        public Dictionary<string, List<string>> UsersInRooms { get { return _usersInRooms; } }
        public string DefaultRoomName { get { return "default"; } }

        public ChatServer(IServerBus serverBus, IDisplay display)
        {
            _serverBus = serverBus;
            _display = display;
        }

        public void Initialize()
        {
            _usersInRooms.Add(DefaultRoomName, new List<string>());
            _serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(SessionHandler);
        }

        internal OpenSessionResponse SessionHandler(OpenSessionRequest request)
        {
            var isLogged = false;
            if (!_users.Contains(request.UserName))
            {
                isLogged = true;
                _users.Add(request.UserName);
                _usersInRooms[DefaultRoomName].Add(request.UserName);
                _display.Print("user '" + request.UserName + "' logged");
            }
            return new OpenSessionResponse { IsLogged = isLogged };
        }

        internal void SwitchRoomHandler(JoinRoomRequest request)
        {
            var defaultRoomUsers = _usersInRooms[DefaultRoomName];
            if (defaultRoomUsers.Contains(request.Token)) defaultRoomUsers.RemoveAt(defaultRoomUsers.IndexOf(request.Token));

            if (!_usersInRooms.ContainsKey(request.RoomName)) _usersInRooms.Add(request.RoomName, new List<string>());
            _usersInRooms[request.RoomName].Add(request.Token);
        }
    }
}
