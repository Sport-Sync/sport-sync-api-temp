using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IFriendshipRequestRepository
{
    Task<Maybe<FriendshipRequest>> GetByIdAsync(Guid friendshipRequestId, CancellationToken cancellationToken);
    Task<List<FriendshipRequest>> GetAllPendingForUserIdAsync(Guid userId);
    void Insert(FriendshipRequest friendshipRequest);
    void InsertRange(IReadOnlyCollection<FriendshipRequest> entities);
}