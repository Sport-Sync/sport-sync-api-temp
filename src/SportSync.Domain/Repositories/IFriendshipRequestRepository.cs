using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IFriendshipRequestRepository
{
    Task<Maybe<FriendshipRequest>> GetByIdAsync(Guid friendshipRequestId, CancellationToken cancellationToken);
    Task<List<FriendshipRequest>> GetPendingForFriendIdAsync(Guid friendId, CancellationToken cancellationToken);
    Task<List<FriendshipRequest>> GetAllPendingForUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Maybe<FriendshipRequest>> GetPendingForUsersAsync(Guid firstUserId, Guid secondUserId, CancellationToken cancellationToken);
    void Insert(FriendshipRequest friendshipRequest);
    void InsertRange(IReadOnlyCollection<FriendshipRequest> entities);
    void Remove(FriendshipRequest friendshipRequest);
}