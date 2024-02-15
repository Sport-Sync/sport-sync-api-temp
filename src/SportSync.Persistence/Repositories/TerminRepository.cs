using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Persistence.Repositories;

public class TerminRepository : QueryableGenericRepository<Termin, TerminType>, ITerminRepository
{
    public TerminRepository(IDbContext dbContext) : base(dbContext, TerminType.PropertySelector)
    {
    }

    public override async Task<Maybe<Termin>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Maybe<Termin>.From(await DbContext.Set<Termin>()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken));
    }

    public async Task<List<Player>> GetPlayers(Guid id, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Player>()
            .Include(p => p.User)
            .Where(t => t.TerminId == id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<(Termin LastTermin, int PendingTerminsCount)>> GetLastRepeatableTermins()
    {
        var today = DateTime.Today;

        var lastTerminsByEvent = await DbContext.Set<Termin>()
            .Include(t => t.Schedule)
            .Include(t => t.Players)
            .Where(t => t.Date > today && t.Schedule.RepeatWeekly)
            .GroupBy(t => t.EventId)
            .Select(g => new { EventId = g.Key, LastTermin = g.OrderByDescending(t => t.Date).First(), Count = g.Count() })
            .ToListAsync();

        return lastTerminsByEvent.Select(x => (x.LastTermin, x.Count)).ToList();
    }

    public async Task<List<Termin>> GetAnnouncedTermins(DateTime date, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Termin>()
            .Include(t => t.Announcements)
            .Include(t => t.Players)
                .ThenInclude(p => p.User)
            .Where(t => t.Date.Date == date.Date && t.Announcements.Any())
            .ToListAsync(cancellationToken);
    }
}