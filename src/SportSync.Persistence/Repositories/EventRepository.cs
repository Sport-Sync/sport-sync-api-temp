using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

internal sealed class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<List<EventSchedule>> GetAllRepeatableEventSchedules()
    {
        return DbContext.Set<EventSchedule>()                                      
            .Include(es => es.Event.Termins)
            .Include(es => es.Event.Members)
            .Where(es => es.Event.Status == EventStatus.Active)
            .Where(es => es.RepeatWeekly)
            .ToListAsync();
    }
}
