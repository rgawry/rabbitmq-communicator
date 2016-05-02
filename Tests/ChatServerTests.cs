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
            var request = new OpenSessionRequest { UserName = userName };

            Assert.That(chatServer.Handler(request).IsLogged, Is.True);
            Assert.That(chatServer.Handler(request).IsLogged, Is.False);
        }
    }
}
