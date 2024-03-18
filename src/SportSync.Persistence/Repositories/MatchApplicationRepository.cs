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

    public async Task<List<MatchApplication>> GetByMatchIdAsync(Guid matchId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<MatchApplication>()
            .Where(t => t.MatchId == matchId)
            .ToListAsync(cancellationToken);
    }
}