using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

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

    public async Task<List<UserType>> GetMutualFriends(User firstUser, User secondUser, List<User> firstUserFriends = null, CancellationToken cancellationToken = default)
    {
        firstUserFriends ??= await _userRepository.GetByIdsAsync(firstUser.Friends.ToList(), cancellationToken);

        var mutualFriends = firstUserFriends.Where(friend => secondUser.Friends.Contains(friend.Id))
            .Select(x => new UserType(x))
            .ToList();

        return mutualFriends;
    }
}