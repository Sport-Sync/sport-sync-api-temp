using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Services;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUserProfile;

public class GetUserProfileRequestHandler : IRequestHandler<GetUserProfileInput, UserProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserProfileImageService _profileImageService;
    private readonly IUserProfileImageService _userProfileImageService;
    private readonly IFriendshipInformationService _friendshipInformationService;

    public GetUserProfileRequestHandler(
        IUserRepository userRepository,
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IUserProfileImageService profileImageService,
        IFriendshipInformationService friendshipInformationService,
        IUserProfileImageService userProfileImageService)
    {
        _userRepository = userRepository;
        _friendshipRequestRepository = friendshipRequestRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _profileImageService = profileImageService;
        _friendshipInformationService = friendshipInformationService;
        _userProfileImageService = userProfileImageService;
    }


    public async Task<UserProfileResponse> Handle(GetUserProfileInput request, CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        var maybeCurrentUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeCurrentUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var user = maybeUser.Value;
        var currentUser = maybeCurrentUser.Value;

        var currentUserFriends = await _userRepository.GetByIdsAsync(currentUser.Friends.ToList(), cancellationToken);

        var friendshipInformation = await _friendshipInformationService.GetFriendshipInformationForCurrentUser(currentUser, user, currentUserFriends, _friendshipRequestRepository, _userProfileImageService, cancellationToken);

        var userProfileType = new UserProfileType(user, friendshipInformation);

        await _profileImageService.PopulateImageUrl(userProfileType);

        var userProfileResponse = new UserProfileResponse
        {
            User = userProfileType
        };

        return userProfileResponse;
    }
}