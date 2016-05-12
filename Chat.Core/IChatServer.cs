namespace Chat
{
    public interface IChatServer
    {
        OpenSessionResponse SessionHandler(OpenSessionRequest request);
        void SwitchRoomHandler(JoinRoomRequest request);
    }
}
