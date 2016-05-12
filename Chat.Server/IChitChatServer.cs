namespace Chat
{
    public interface IChitChatServer
    {
        OpenSessionResponse SessionHandler(OpenSessionRequest request);
        void SwitchRoomHandler(JoinRoomRequest request);
    }
}
