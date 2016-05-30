using Castle.Core;

namespace Chat
{
    public sealed class ChatServer : IInitializable
    {
        private IServerBus _serverBus;
        private ISessionHandler _sessionHandler;

        public ChatServer(IServerBus serverBus, ISessionHandler sessionHandler)
        {
            _serverBus = serverBus;
            _sessionHandler = sessionHandler;
        }

        public void Initialize()
        {
            _serverBus.AddHandler<JoinRoomRequest>(_sessionHandler.JoinRoom);
            _serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(_sessionHandler.Login);
        }
    }
}
