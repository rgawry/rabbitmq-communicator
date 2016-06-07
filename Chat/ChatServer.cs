using Castle.Core;

namespace Chat
{
    public sealed class ChatServer : IInitializable
    {
        private IServerBus _serverBus;
        private IRequestResponseService<OpenSessionRequest, OpenSessionResponse> _sessionService;
        private IRequestService<JoinRoomRequest> _joinRoomService;
        private IRequestResponseService<TokenRequest, TokenResponse> _tokenService;

        public ChatServer(IServerBus serverBus, IRequestResponseService<OpenSessionRequest, OpenSessionResponse> sessionService, IRequestService<JoinRoomRequest> joinRoomService, IRequestResponseService<TokenRequest, TokenResponse> tokenService)
        {
            _serverBus = serverBus;
            _sessionService = sessionService;
            _joinRoomService = joinRoomService;
            _tokenService = tokenService;
        }

        public void Initialize()
        {
            _serverBus.AddHandler<TokenRequest, TokenResponse>(_tokenService.Handle);
            _serverBus.AddHandler<JoinRoomRequest>(_joinRoomService.Handle);
            _serverBus.AddHandler<OpenSessionRequest, OpenSessionResponse>(_sessionService.Handle);
        }
    }
}
