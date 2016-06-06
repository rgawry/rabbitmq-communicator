using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    public class TokenServiceTest
    {
        [Test]
        public void GetToken_ShouldReturnNewToken()
        {
            var request = new TokenRequest();
            var ts = new TokenService();

            var response = ts.GetToken(request);

            Assert.That(!string.IsNullOrWhiteSpace(response.Token), Is.True);
        }
    }
}
