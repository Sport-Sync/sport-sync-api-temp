using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IFriendshipRequestRepository
{
    Task<bool> CheckForPendingFriendshipRequestAsync(User user, User friend);
    void Insert(FriendshipRequest friendshipRequest);
}