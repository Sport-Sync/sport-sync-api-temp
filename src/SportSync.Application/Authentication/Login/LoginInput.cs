using SportSync.Application.Core.Abstractions.Common;

namespace SportSync.Application.Authentication.Login;

public class LoginInput : IInput<TokenResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
}