using System.Collections.Generic;

namespace Chat
{
    public class RoomService : IRoomService
    {
        private string _defaultRoomName = "default";
        internal SynchronizedCollection<User> _users = new SynchronizedCollection<User>();

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public string GetDefaultRoomName()
        {
            return _defaultRoomName;
        }

        public bool IsUserLoggedIn(string token)
        {
            lock (_users.SyncRoot)
            {
                for (int i = 0; i < _users.Count; i++)
                {
                    if (token == _users[i].Token) return true;
                }
                return false;
            }
        }

        public bool IsUserNameTaken(string userName)
        {
            lock (_users.SyncRoot)
            {
                for (int i = 0; i < _users.Count; i++)
                {
                    if (userName == _users[i].Name) return true;
                }
                return false;
            }
        }

        public void SetUserRoom(string token, string roomName)
        {
            lock (_users.SyncRoot)
            {
                for (int i = 0; i < _users.Count; i++)
                {
                    if (token == _users[i].Token) _users[i].Room = roomName;
                }
            }
        }
    }
}
