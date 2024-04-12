using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetCurrentUser;

public class GetCurrentUserRequestHandler : IRequestHandler<UserType>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserRequestHandler(IUserIdentifierProvider userIdentifierProvider, IUserRepository userRepository)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
    }

    public async Task<UserType> Handle(CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        var user = new UserType(maybeUser.Value);
        
        return user;
    }
}