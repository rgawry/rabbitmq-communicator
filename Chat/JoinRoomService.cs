namespace Chat
{
    public class JoinRoomService : IRequestService<JoinRoomRequest>
    {
        private IRoomService _roomService;

        public JoinRoomService(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public void Handle(JoinRoomRequest request)
        {
            if (_roomService.IsUserLoggedIn(request.Token)) _roomService.SetUserRoom(request.Token, request.RoomName);
        }
    }
}
