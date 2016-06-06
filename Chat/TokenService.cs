using System;

namespace Chat
{
    public class TokenService : IService<TokenRequest, TokenResponse>
    {
        public TokenResponse Handle(TokenRequest request)
        {
            return new TokenResponse { Token = Guid.NewGuid().ToString() };
        }
    }
}
