namespace SportSync.Application.Authentication.Login;

public class LoginInput : IRequest<TokenResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
}