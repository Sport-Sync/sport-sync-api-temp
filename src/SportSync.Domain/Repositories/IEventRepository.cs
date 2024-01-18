using SportSync.Domain.DtoTypes;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IEventRepository
{
    void Insert(Event @event);
    IQueryable<TerminType> GetTermins(Guid userId, DateTime dateTime);
}
