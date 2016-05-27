using NSubstitute;
using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    public class ChatServerTest
    {
        [Test]
        public void ShouldUserLogIn()
        {
            var chatServer = new ChatServer(Substitute.For<IServerBus>(), Substitute.For<IDisplay>());
            chatServer.Initialize();
            var request = new OpenSessionRequest { UserName = "login1" };

            Assert.That(chatServer.SessionHandler(request).IsLogged, Is.True);
            Assert.That(chatServer.SessionHandler(request).IsLogged, Is.False);
            Assert.That(chatServer.UsersInRooms[chatServer.DefaultRoomName].Contains(request.UserName), Is.True);
        }

        [Test]
        public void ShouldUserSwitchRoom()
        {
            var chatServer = new ChatServer(Substitute.For<IServerBus>(), Substitute.For<IDisplay>());
            chatServer.Initialize();
            var request = new JoinRoomRequest { RoomName = "testRoomName", Token = "login1" };

            chatServer.SwitchRoomHandler(request);

            Assert.That(!chatServer.UsersInRooms[chatServer.DefaultRoomName].Contains(request.Token), Is.True);
            Assert.That(chatServer.UsersInRooms[request.RoomName].Contains(request.Token), Is.True);
        }

        [Test]
        public void ShouldNotJoinTwiceSameRoom()
        {
            var chatServer = new ChatServer(Substitute.For<IServerBus>(), Substitute.For<IDisplay>());
            chatServer.Initialize();
            var request = new JoinRoomRequest { RoomName = "testRoomName", Token = "login1" };

            chatServer.SwitchRoomHandler(request);
            chatServer.SwitchRoomHandler(request);

            Assert.That(!chatServer.UsersInRooms[chatServer.DefaultRoomName].Contains(request.Token), Is.True);
            Assert.That(chatServer.UsersInRooms[request.RoomName].Contains(request.Token), Is.True);
            Assert.That(chatServer.UsersInRooms[request.RoomName].Count, Is.EqualTo(1));
        }
    }
}
