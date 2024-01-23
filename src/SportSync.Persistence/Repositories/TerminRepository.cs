using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Persistence.Repositories;

public class TerminRepository : QueryableGenericRepository<Termin, TerminType>, ITerminRepository
{
    public TerminRepository(IDbContext dbContext) : base(dbContext, TerminType.PropertySelector)
    {
    }
}