using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface ITerminApplicationRepository
{
    Task<Maybe<TerminApplication>> GetByIdAsync(Guid terminApplicationId, CancellationToken cancellationToken);
    void Insert(TerminApplication terminApplication);
}