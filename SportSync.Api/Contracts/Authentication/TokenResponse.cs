namespace sport_sync.Contracts.Authentication;

public class TokenResponse
{
    public TokenResponse(string token) => Token = token;

    public string Token { get; }
}