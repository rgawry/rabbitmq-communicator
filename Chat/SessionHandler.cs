using System;
using System.Collections.Generic;

namespace Chat
{
    public class SessionHandler : ISessionHandler
    {
        private string _defaultRoomName = "default";
        private SynchronizedCollection<User> _users = new SynchronizedCollection<User>();

        public OpenSessionResponse Login(OpenSessionRequest request)
        {
            var result = new OpenSessionResponse();
            var user = new User { Name = request.UserName, Token = request.Token, Room = _defaultRoomName };

            if (Contains(user))
            {
                result.IsLogged = false;
                return result;
            }
            _users.Add(user);
            result.IsLogged = true;
            return result;
        }

        public void JoinRoom(JoinRoomRequest request)
        {
            var userIndex = IndexOf(new User { Token = request.Token });
            if (userIndex == -1) return;
            _users[userIndex].Room = request.RoomName;
        }

        public TokenResponse NewToken(TokenRequest request)
        {
            return new TokenResponse { Token = Guid.NewGuid().ToString() };
        }

        public string GetUserRoom(string token)
        {
            var userIndex = IndexOf(new User { Token = token });
            if (userIndex == -1) return string.Empty;
            return _users[userIndex].Room;
        }

        bool Contains(User user)
        {
            lock (_users.SyncRoot)
            {
                for (int i = 0; i < _users.Count; i++)
                {
                    if (user.Name == _users[i].Name || user.Token == _users[i].Token) return true;
                }
                return false;
            }
        }

        int IndexOf(User user)
        {
            lock (_users.SyncRoot)
            {
                for (int i = 0; i < _users.Count; i++)
                {
                    if (user.Token == _users[i].Token) return i;
                }
                return -1;
            }
        }
    }
}
