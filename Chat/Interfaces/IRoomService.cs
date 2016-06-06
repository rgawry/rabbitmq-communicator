namespace Chat
{
    public interface IRoomService
    {
        bool IsUserLoggedIn(string token);
        void SetUserRoom(string token, string roomName);
        bool IsUserNameTaken(string userName);
        void AddUser(User user);
        string GetDefaultRoomName();
    }
}
