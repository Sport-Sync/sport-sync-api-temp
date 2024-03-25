﻿using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Types;

namespace SportSync.Domain.Repositories;

public interface IFriendshipRequestRepository : IQueryableRepository<FriendshipRequest, FriendshipRequestType>
{
    Task<Maybe<FriendshipRequest>> GetByIdAsync(Guid friendshipRequestId, CancellationToken cancellationToken);
    Task<List<FriendshipRequest>> GetAllPendingForUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Maybe<FriendshipRequest>> GetPendingForUsersAsync(Guid firstUserId, Guid secondUserId, CancellationToken cancellationToken);
    void Insert(FriendshipRequest friendshipRequest);
    void InsertRange(IReadOnlyCollection<FriendshipRequest> entities);
    void Remove(FriendshipRequest friendshipRequest);
}