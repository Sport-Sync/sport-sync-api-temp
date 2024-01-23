using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
namespace SportSync.Domain.Repositories;

public interface IEventRepository
{
    void Insert(Event @event);
    Task<(Event Event, Termin LastTermin, EventSchedule Schedule, int PendingTerminsCount)[]> GetAllRepeatableEventSchedules();
    Task<List<Event>> GetAllByIds(List<Guid> ids);
    Task<Maybe<Event>> GetByIdAsync(Guid id);
}
