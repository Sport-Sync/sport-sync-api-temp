using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IFriendshipRequestRepository
{
    Task<Maybe<FriendshipRequest>> GetByIdAsync(Guid friendshipRequestId, CancellationToken cancellationToken);
    Task<bool> CheckForPendingFriendshipRequestAsync(User user, User friend);
    void Insert(FriendshipRequest friendshipRequest);
}