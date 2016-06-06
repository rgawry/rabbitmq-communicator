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
            var tokenService = new TokenService();

            var response = tokenService.GetToken(request);

            Assert.That(!string.IsNullOrWhiteSpace(response.Token), Is.True);
        }
    }
}
