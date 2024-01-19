using SportSync.Domain.DtoTypes;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface ITerminRepository : IQueryableRepository<Termin, TerminType>
{
}