using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Domain.Services;

public class FriendshipService
{
    private readonly IUserRepository _userRepository;

    public FriendshipService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task CreateFriendshipAsync(FriendshipRequest friendshipRequest, CancellationToken cancellationToken)
    {
        if (friendshipRequest.Rejected)
        {
            throw new DomainException(DomainErrors.FriendshipRequest.AlreadyRejected);
        }

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(friendshipRequest.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        Maybe<User> maybeFriend = await _userRepository.GetByIdAsync(friendshipRequest.FriendId, cancellationToken);

        if (maybeFriend.HasNoValue)
        {
            throw new DomainException(DomainErrors.FriendshipRequest.FriendNotFound);
        }

        User user = maybeUser.Value;
        User friend = maybeFriend.Value;

        user.AddFriend(friend);
    }
}