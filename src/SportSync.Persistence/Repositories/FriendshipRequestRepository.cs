using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

public class FriendshipRequestRepository : GenericRepository<FriendshipRequest>, IFriendshipRequestRepository
{
    public FriendshipRequestRepository(IDbContext dbContext)
        : base(dbContext)
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