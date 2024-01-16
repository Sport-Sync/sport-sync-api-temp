using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

internal sealed class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(IDbContext dbContext) 
        : base(dbContext)
    {
    }
}