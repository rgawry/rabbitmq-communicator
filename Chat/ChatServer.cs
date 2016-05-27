using Castle.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Chat
{
    public sealed class ChatServer : IInitializable
    {
        private IDisplay _display;
        private IServerBus _serverBus;
        private SynchronizedCollection<string> _users = new SynchronizedCollection<string>();
        private ConcurrentDictionary<string, List<string>> _usersInRooms = new ConcurrentDictionary<string, List<string>>();

        public ConcurrentDictionary<string, List<string>> UsersInRooms { get { return _usersInRooms; } }
        public string DefaultRoomName { get { return "default"; } }

        public ChatServer(IServerBus serverBus, IDisplay display)
        {
            _serverBus = serverBus;
            _display = display;
        }

        public void Initialize()
        {
            _usersInRooms.TryAdd(DefaultRoomName, new List<string>());
            _serverBus.AddHandler<JoinRoomRequest>(SwitchRoomHandler);
            _serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(SessionHandler);
        }

        internal OpenSessionResponse SessionHandler(OpenSessionRequest request)
        {
            var isLogged = false;
            var result = string.Empty;
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
            var defaultRoomUsers = new List<string>();
            if (_usersInRooms.TryGetValue(DefaultRoomName, out defaultRoomUsers))
            {
                if (defaultRoomUsers.Contains(request.Token)) defaultRoomUsers.RemoveAt(defaultRoomUsers.IndexOf(request.Token));
            }

            if (!_usersInRooms.TryAdd(request.RoomName, new List<string>())) return;
            _usersInRooms[request.RoomName].Add(request.Token);
            _display.Print("user '" + request.Token + "' switched from default room to " + request.RoomName);
        }
    }
}
