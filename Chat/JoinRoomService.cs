namespace Chat
{
    public class JoinRoomService : IJoinRoomService
    {
        private IRoomService _roomService;

        public JoinRoomService(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public void JoinRoom(JoinRoomRequest request)
        {
            if (_roomService.IsUserLoggedIn(request.Token)) _roomService.SetUserRoom(request.Token, request.RoomName);
        }
    }
}
