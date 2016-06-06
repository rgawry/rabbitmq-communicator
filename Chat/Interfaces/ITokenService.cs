namespace Chat
{
    public interface ITokenService
    {
        TokenResponse GetToken(TokenRequest request);
    }
}
