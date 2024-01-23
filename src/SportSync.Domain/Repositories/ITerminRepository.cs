using SportSync.Domain.Entities;
using SportSync.Domain.Types;

namespace SportSync.Domain.Repositories;

public interface ITerminRepository : IQueryableRepository<Termin, TerminType>
{
    void InsertRange(IReadOnlyCollection<Termin> termins);
    Task<List<(Termin LastTermin, int PendingTerminsCount)>> GetLastRepeatableTermins();
}