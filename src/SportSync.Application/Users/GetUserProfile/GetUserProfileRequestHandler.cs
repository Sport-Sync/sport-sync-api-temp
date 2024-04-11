using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUserProfile;

public class GetUserProfileRequestHandler : IRequestHandler<GetUserProfileInput, UserProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IBlobStorageService _blobStorageService;

    public GetUserProfileRequestHandler(
        IUserRepository userRepository,
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IBlobStorageService blobStorageService)
    {
        _userRepository = userRepository;
        _friendshipRequestRepository = friendshipRequestRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _blobStorageService = blobStorageService;
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

        var friendWithCurrentUser = currentUser.IsFriendWith(user);

        var maybePendingFriendshipRequest = friendWithCurrentUser ? Maybe<FriendshipRequest>.None :
            await _friendshipRequestRepository.GetPendingForUsersAsync(currentUser.Id, user.Id, cancellationToken);

        PendingFriendshipRequestType? pendingFriendshipRequestType = maybePendingFriendshipRequest.HasNoValue
            ? null
            : PendingFriendshipRequestType.Create(maybePendingFriendshipRequest.Value.Id, maybePendingFriendshipRequest.Value.IsSender(currentUser.Id));

        var mutualFriendIds = user.Friends.Where(friendId => currentUser.Friends.Contains(friendId)).ToList();
        var mutualFriends =
            mutualFriendIds.Any() ?
            await _userRepository.GetByIdsAsync(mutualFriendIds, cancellationToken) :
            new List<User>();

        List<UserType> mutualFriendList = new();

        foreach (var mutualFriend in mutualFriends)
        {
            var profileImage = mutualFriend.HasProfileImage ? await _blobStorageService.GetProfileImageUrl(mutualFriend.Id) : null;
            mutualFriendList.Add(new UserType(mutualFriend, profileImage));
        }

        var userProfileImage = user.HasProfileImage ? await _blobStorageService.GetProfileImageUrl(user.Id) : null;

        var userProfileResponse = new UserProfileResponse(user, userProfileImage)
        {
            IsFriendWithCurrentUser = friendWithCurrentUser,
            PendingFriendshipRequest = pendingFriendshipRequestType,
            MutualFriends = mutualFriendList
        };

        return userProfileResponse;
    }
}