namespace Chat
{
    public class SessionService : ISessionService
    {
        private IRoomService _roomService;

        public SessionService(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public OpenSessionResponse Login(OpenSessionRequest request)
        {
            var result = new OpenSessionResponse();
            var user = new User { Name = request.UserName, Token = request.Token, Room = _roomService.GetDefaultRoomName() };
            result.IsLogged = !_roomService.IsUserLoggedIn(request.Token) && !_roomService.IsUserNameTaken(request.UserName);
            if (result.IsLogged) _roomService.AddUser(user);
            return result;
        }
    }
}
