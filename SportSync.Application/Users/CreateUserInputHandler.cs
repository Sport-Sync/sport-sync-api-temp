using SportSync.Application.Authentication;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Users;

public class CreateUserInputHandler : IInputHandler<CreateUserInput, TokenResponse>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserRepository _userRepository;

    public CreateUserInputHandler(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    public Task<TokenResponse> Handle(CreateUserInput input)
    {
    }
}