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

    public async Task<bool> CheckForPendingFriendshipRequestAsync(User user, User friend)
    {
        return await DbContext.Set<FriendshipRequest>()
            .AnyAsync(friendshipRequest =>
                (friendshipRequest.UserId == user.Id || friendshipRequest.UserId == friend.Id) &&
                (friendshipRequest.FriendId == user.Id || friendshipRequest.FriendId == friend.Id) &&
                friendshipRequest.CompletedOnUtc == null);
    }
}