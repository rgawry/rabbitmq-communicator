using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    public class ChatServerTest
    {
        [Test]
        public void ShouldUserLogIn()
        {
            const string userName = "login1";
            var chatServer = new ChatServer();
            chatServer.Initialize();
            var request = new OpenSessionRequest { UserName = userName };

            Assert.That(chatServer.SessionHandler(request).IsLogged, Is.True);
            Assert.That(chatServer.SessionHandler(request).IsLogged, Is.False);
            Assert.That(chatServer.UsersInRooms[chatServer.DefaultRoomName].Contains(request.UserName), Is.True);
        }

        [Test]
        public void ShouldUserSwitchRoom()
        {
            var chatServer = new ChatServer();
            chatServer.Initialize();
            var request = new JoinRoomRequest { RoomName = "testRoomName", Token = "login1" };

            chatServer.SwitchRoomHandler(request);

            Assert.That(!chatServer.UsersInRooms[chatServer.DefaultRoomName].Contains(request.Token), Is.True);
            Assert.That(chatServer.UsersInRooms[request.RoomName].Contains(request.Token), Is.True);
        }
    }
}
