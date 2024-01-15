using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IEventRepository : IRepository<Event>
{
    void Insert(Event @event);
}