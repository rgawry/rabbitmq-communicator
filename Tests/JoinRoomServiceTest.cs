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
            var rs = Substitute.For<IRoomService>();
            var jrs = new JoinRoomService(rs);
            rs.IsUserLoggedIn(Arg.Any<string>()).Returns(true);

            jrs.JoinRoom(request);
            rs.Received().SetUserRoom(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
