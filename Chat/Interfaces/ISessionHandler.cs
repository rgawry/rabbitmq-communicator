namespace Chat
{
    public interface ISessionHandler
    {
        OpenSessionResponse Login(OpenSessionRequest request);
        void JoinRoom(JoinRoomRequest request);
    }
}