using SportSync.Application.Authentication;

namespace SportSync.Application.Users.CreateUser;

public record CreateUserInput : IRequest<TokenResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
}