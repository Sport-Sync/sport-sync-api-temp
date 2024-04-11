using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Core.Services;

public class FriendshipInformationService : IFriendshipInformationService
{
    public async Task<FriendshipInformationType> GetFriendshipInformationForCurrentUser(
        User currentUser,
        User otherUser,
        List<User> currentUserFriends, 
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserProfileImageService userProfileImageService,
        CancellationToken cancellationToken)
    {
        var mutualFriends = currentUserFriends.Where(friend => otherUser.Friends.Contains(friend.Id))
            .Select(x => new UserType(x))
            .ToList();

        await userProfileImageService.PopulateImageUrl(mutualFriends.ToArray());

        var friendWithCurrentUser = currentUser.IsFriendWith(otherUser);

        var maybePendingFriendshipRequest = friendWithCurrentUser ? Maybe<FriendshipRequest>.None :
            await friendshipRequestRepository.GetPendingForUsersAsync(currentUser.Id, otherUser.Id, cancellationToken);

        PendingFriendshipRequestType? pendingFriendshipRequestType = maybePendingFriendshipRequest.HasNoValue
            ? null
            : PendingFriendshipRequestType.Create(maybePendingFriendshipRequest.Value.Id, maybePendingFriendshipRequest.Value.IsSender(currentUser.Id));

        return new FriendshipInformationType(mutualFriends, pendingFriendshipRequestType, friendWithCurrentUser);
    }
}