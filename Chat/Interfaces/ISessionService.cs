namespace Chat
{
    public interface ISessionService
    {
        OpenSessionResponse Login(OpenSessionRequest request);
    }
}
