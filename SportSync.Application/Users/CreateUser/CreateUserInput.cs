using SportSync.Application.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Users.CreateUser;

public record CreateUserInput : IInput<Result<TokenResponse>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
}