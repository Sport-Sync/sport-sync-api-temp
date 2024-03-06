using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
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
}