using System;

namespace Chat
{
    public class TokenService : ITokenService
    {
        public TokenResponse GetToken(TokenRequest request)
        {
            return new TokenResponse { Token = Guid.NewGuid().ToString() };
        }
    }
}
