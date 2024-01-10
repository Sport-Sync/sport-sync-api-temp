using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Services;

namespace SportSync.Application.Authentication.Login;

public class LoginInputHandler : IInputHandler<LoginInput, TokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashChecker _passwordHashChecker;
    private readonly IJwtProvider _jwtProvider;

    public LoginInputHandler(IUserRepository userRepository, IPasswordHashChecker passwordHashChecker, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHashChecker = passwordHashChecker;
        _jwtProvider = jwtProvider;
    }

    public async Task<TokenResponse> Handle(LoginInput request, CancellationToken cancellationToken)
    {
        Maybe<User> maybeUser = await _userRepository.GetByEmailAsync(request.Email);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.Authentication.InvalidEmailOrPassword);
        }

        var user = maybeUser.Value;

        var passwordValid = user.VerifyPasswordHash(request.Password, _passwordHashChecker);

        if (!passwordValid)
        {
            throw new DomainException(DomainErrors.Authentication.InvalidEmailOrPassword);
        }

        var token = _jwtProvider.Create(user);

        return new TokenResponse(token);
    }
}