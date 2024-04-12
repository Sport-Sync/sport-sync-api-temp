using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Services;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetCurrentUser;

public class GetCurrentUserRequestHandler : IRequestHandler<UserType>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileImageService _userProfileImageService;

    public GetCurrentUserRequestHandler(IUserIdentifierProvider userIdentifierProvider, IUserRepository userRepository, IUserProfileImageService userProfileImageService)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _userProfileImageService = userProfileImageService;
    }

    public async Task<UserType> Handle(CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        var user = new UserType(maybeUser.Value);

        await _userProfileImageService.PopulateImageUrl(user);

        return user;
    }
}