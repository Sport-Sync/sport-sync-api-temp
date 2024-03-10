using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Persistence.Repositories;

public class FriendshipRequestRepository : QueryableGenericRepository<FriendshipRequest, FriendshipRequestType>, IFriendshipRequestRepository
{
    public FriendshipRequestRepository(IDbContext dbContext)
        : base(dbContext, FriendshipRequestType.PropertySelector)
    {
    }
    
    public async Task<List<FriendshipRequest>> GetAllPendingForUserIdAsync(Guid userId)
    {
        return await DbContext.Set<FriendshipRequest>()
            .Where(friendshipRequest =>
                (friendshipRequest.UserId == userId || friendshipRequest.FriendId == userId) &&
                friendshipRequest.CompletedOnUtc == null)
            .ToListAsync();
    }

    public override async Task<Maybe<FriendshipRequest>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Maybe<FriendshipRequest>.From(await DbContext.Set<FriendshipRequest>()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken));
    }
}