using Castle.Core;

namespace Chat
{
    public sealed class ChatServer : IInitializable
    {
        private IServerBus _serverBus;
        private ISessionService _sessionService;
        private IJoinRoomService _joinRoomService;
        private IService<TokenRequest, TokenResponse> _tokenService;

        public ChatServer(IServerBus serverBus, ISessionService sessionService, IJoinRoomService joinRoomService, IService<TokenRequest, TokenResponse> tokenService)
        {
            _serverBus = serverBus;
            _sessionService = sessionService;
            _joinRoomService = joinRoomService;
            _tokenService = tokenService;
        }

        public void Initialize()
        {
            _serverBus.AddHandler<TokenRequest, TokenResponse>(_tokenService.Handle);
            _serverBus.AddHandler<JoinRoomRequest>(_joinRoomService.JoinRoom);
            _serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(_sessionService.Login);
        }
    }
}
