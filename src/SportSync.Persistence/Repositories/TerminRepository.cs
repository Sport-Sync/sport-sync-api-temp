using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Persistence.Repositories;

public class TerminRepository : QueryableGenericRepository<Termin, TerminType>, ITerminRepository
{
    public TerminRepository(IDbContext dbContext) : base(dbContext, TerminType.PropertySelector)
    {
    }

    public async Task<List<(Termin LastTermin, int PendingTerminsCount)>> GetLastRepeatableTermins()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var lastTerminsByEvent = await DbContext.Set<Termin>()
            .Include(t => t.Schedule)
            .Include(t => t.Players)
            .Where(t => t.Date > today && t.Schedule.RepeatWeekly)
            .GroupBy(t => t.EventId)
            .Select(g => new { EventId = g.Key, LastTermin = g.OrderByDescending(t => t.Date).First(), Count = g.Count() })
            .ToListAsync();

        return lastTerminsByEvent.Select(x => (x.LastTermin, x.Count)).ToList();
    }
}