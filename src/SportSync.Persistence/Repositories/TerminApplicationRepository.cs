using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

public class TerminApplicationRepository : GenericRepository<TerminApplication>, ITerminApplicationRepository
{
    public TerminApplicationRepository(IDbContext dbContext) 
        : base(dbContext)
    {
    }
}