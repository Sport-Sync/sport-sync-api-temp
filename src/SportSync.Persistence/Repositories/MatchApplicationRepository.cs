using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

public class MatchApplicationRepository : GenericRepository<MatchApplication>, IMatchApplicationRepository
{
    public MatchApplicationRepository(IDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<List<MatchApplication>> GetPendingByMatchId(Guid matchId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<MatchApplication>()
            .Where(t => t.MatchId == matchId && t.CompletedOnUtc == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MatchApplication>> GetPendingByMatchIdWithIncludedUser(Guid matchId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<MatchApplication>()
            .Where(t => t.MatchId == matchId && t.CompletedOnUtc == null)
            .Include(m => m.AppliedByUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MatchApplication>> GetPendingByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<MatchApplication>()
            .Where(t => t.AppliedByUserId == userId && t.CompletedOnUtc == null)
            .ToListAsync(cancellationToken);
    }
}