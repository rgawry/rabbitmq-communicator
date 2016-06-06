using Castle.Core;

namespace Chat
{
    public sealed class ChatServer : IInitializable
    {
        private IServerBus _serverBus;
        private ISessionService _sessionService;
        private IJoinRoomService _joinRoomService;
        private ITokenService _tokenService;

        public ChatServer(IServerBus serverBus, ISessionService sessionService, IJoinRoomService joinRoomService, ITokenService tokenService)
        {
            _serverBus = serverBus;
            _sessionService = sessionService;
            _joinRoomService = joinRoomService;
            _tokenService = tokenService;
        }

        public void Initialize()
        {
            _serverBus.AddHandler<TokenRequest, TokenResponse>(_tokenService.GetToken);
            _serverBus.AddHandler<JoinRoomRequest>(_joinRoomService.JoinRoom);
            _serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(_sessionService.Login);
        }
    }
}
