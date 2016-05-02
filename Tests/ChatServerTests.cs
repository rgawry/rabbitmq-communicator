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
            var server = new ChitChatServer();
            var request = new OpenSessionRequest { UserName = userName };

            Assert.That(server.Handler(request).IsLogged, Is.True);
            Assert.That(server.Handler(request).IsLogged, Is.False);
        }
    }
}
