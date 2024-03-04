using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Types;

namespace SportSync.Domain.Repositories;

public interface ITerminRepository : IQueryableRepository<Termin, TerminType>
{
    Task<Maybe<Termin>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Player>> GetPlayers(Guid id, CancellationToken cancellationToken);
    Task<List<EventMember>> GetAdmins(Guid terminId, CancellationToken cancellationToken);
    Task<List<(Termin LastTermin, int PendingTerminsCount)>> GetLastRepeatableTermins();
    Task<List<Termin>> GetAnnouncedTermins(DateTime date, CancellationToken cancellationToken);
    void InsertRange(IReadOnlyCollection<Termin> termins);
}