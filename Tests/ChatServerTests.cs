using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    public class ChatServerTests
    {
        [Test]
        public void ShouldGivenUserLogIn()
        {
            const string userName = "login1";
            var chatServer = new ChitChatServer();
            chatServer.Init();
            var request = new OpenSessionRequest { UserName = userName };

            Assert.That(chatServer.SessionHandler(request).IsLogged, Is.True);
            Assert.That(chatServer.SessionHandler(request).IsLogged, Is.False);
        }

        [Test]
        public void ShouldSwitchRoom()
        {
            var chatServer = new ChitChatServer();
            chatServer.Init();
            var request = new JoinRoomRequest { RoomName = "testRoomName", Token = "login1" };

            Assert.That();
        }
    }
}
