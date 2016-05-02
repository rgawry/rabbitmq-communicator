using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    public class ServerTests
    {
        [Test]
        public void ShouldGivenUserLogIn()
        {
            const string userName = "login1";
            var server = new ChitChatServer();

            var req = new Messaging.OpenSessionRequest { UserName = userName };
            Assert.That(server.Handler(req).IsLogged, Is.True);
            Assert.That(server.Handler(req).IsLogged, Is.False);
        }
    }
}
