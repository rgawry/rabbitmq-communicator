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
            var roomServiceStub = Substitute.For<IRoomService>();
            var joinRoomService = new JoinRoomService(roomServiceStub);
            roomServiceStub.IsUserLoggedIn(Arg.Any<string>()).Returns(true);

            joinRoomService.JoinRoom(request);
            roomServiceStub.Received().SetUserRoom(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
