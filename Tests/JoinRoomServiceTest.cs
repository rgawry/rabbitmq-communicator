using NSubstitute;
using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    class JoinRoomServiceTest
    {
        [Test]
        public void JoinRoom_UserShouldSwitchRoom()
        {
            var request = new JoinRoomRequest { RoomName = Arg.Any<string>(), Token = Arg.Any<string>() };
            var roomServiceMock = Substitute.For<IRoomService>();
            var joinRoomService = new JoinRoomService(roomServiceMock);
            roomServiceMock.IsUserLoggedIn(Arg.Any<string>()).Returns(true);

            joinRoomService.Handle(request);
            roomServiceMock.Received().SetUserRoom(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
