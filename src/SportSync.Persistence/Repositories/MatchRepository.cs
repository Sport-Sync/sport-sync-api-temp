using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Persistence.Repositories;

public class MatchRepository : QueryableGenericRepository<Match, MatchType>, IMatchRepository
{
    public MatchRepository(IDbContext dbContext) : base(dbContext, MatchType.PropertySelector)
    {
    }

    public override async Task<Maybe<Match>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Maybe<Match>.From(await DbContext.Set<Match>()
            .Include(t => t.Players)
                .ThenInclude(p => p.User)
            .Include(t => t.Announcements)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken));
    }

    public async Task<List<Player>> GetPlayers(Guid id, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Player>()
            .Include(p => p.User)
            .Where(t => t.MatchId == id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EventMember>> GetAdmins(Guid matchId, CancellationToken cancellationToken)
    {
        var query = from match in DbContext.Set<Match>()
                    join @event in DbContext.Set<Event>() on match.EventId equals @event.Id
                    join member in DbContext.Set<EventMember>() on @event.Id equals member.EventId
                    where match.Id == matchId && member.IsAdmin
                    select member;


        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<(Match LastMatch, int PendingMatchesCount)>> GetLastRepeatableMatches()
    {
        var today = DateTime.Today;

        var lastMatchesByEvent = await DbContext.Set<Match>()
            .Include(t => t.Schedule)
            .Include(t => t.Players)
            .Where(t => t.Date > today && t.Schedule.RepeatWeekly)
            .GroupBy(t => t.EventId)
            .Select(g => new { EventId = g.Key, LastMatch = g.OrderByDescending(t => t.Date).First(), Count = g.Count() })
            .ToListAsync();

        return lastMatchesByEvent.Select(x => (x.LastMatch, x.Count)).ToList();
    }

    public async Task<List<Match>> GetAnnouncedMatches(DateTime date, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Match>()
            .Include(t => t.Announcements)
            .Include(t => t.Players)
                .ThenInclude(p => p.User)
            .Where(t => t.Date.Date == date.Date && t.Announcements.Any())
            .ToListAsync(cancellationToken);
    }
}