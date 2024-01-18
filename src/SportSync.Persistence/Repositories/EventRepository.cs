using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.DtoTypes;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

internal sealed class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public IQueryable<TerminType> GetTermins(Guid userId, DateTime dateTime)
    {
        var dateonly = DateOnly.FromDateTime(dateTime);

        return DbContext.Set<Termin>()
            .Include(x => x.Players)
            .Where(x => x.Players.Select(x => x.UserId).Contains(userId))
            .Where(x => x.Date == dateonly)
            .Select(x => new TerminType
            {
                Address = x.Address,
                Date = x.Date,
                EndTimeUtc = x.EndTimeUtc,
                StartTimeUtc = x.StartTimeUtc,
                SportType = x.SportType,
                NumberOfPlayersExpected = x.NumberOfPlayersExpected,
                Price = x.Price,
                Notes = x.Notes,
                EventName = x.EventName,
                NumberOfPlayers = x.Players.Count
            });
    }
}