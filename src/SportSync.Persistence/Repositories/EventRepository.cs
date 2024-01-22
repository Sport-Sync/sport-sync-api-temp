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

    public async Task<(Event Event, Termin LastTermin, EventSchedule Schedule, int PendingTerminsCount)[]> GetAllRepeatableEventSchedules()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var query = await (from termin in DbContext.Set<Termin>()
                           join @event in DbContext.Set<Event>()
                               on termin.EventId equals @event.Id
                           join schedule in DbContext.Set<EventSchedule>()
                               on termin.ScheduleId equals schedule.Id
                           where @event.Status == EventStatus.Active && schedule.RepeatWeekly == true && termin.Date > today
                           group new { Termin = termin, Event = @event, Schedule = schedule } by @event.Id into grouped
                           select new
                           {
                               Event = grouped.First().Event,
                               LastTermin = grouped.OrderByDescending(g => g.Termin.Date).First().Termin,
                               Schedule = grouped.First().Schedule,
                               PendingTerminsCount = grouped.Count()
                           }).ToArrayAsync();

        return query.Select(x => (x.Event, x.LastTermin, x.Schedule, x.PendingTerminsCount)).ToArray();
    }

    public async Task<List<Event>> GetAllByIds(List<Guid> ids)
    {
        return await DbContext.Set<Event>()
            .Where(ev => ids.Contains(ev.Id))
            .Include(ev => ev.Termins)
            .ToListAsync();
    }
}
