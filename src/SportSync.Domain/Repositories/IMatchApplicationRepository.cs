using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IMatchApplicationRepository
{
    Task<Maybe<MatchApplication>> GetByIdAsync(Guid matchApplicationId, CancellationToken cancellationToken);
    Task<List<MatchApplication>> GetPendingByMatchId(Guid matchId, CancellationToken cancellationToken);
    Task<List<MatchApplication>> GetPendingByMatchesIds(List<Guid> matchIds, CancellationToken cancellationToken);
    Task<List<MatchApplication>> GetPendingByMatchIdWithIncludedUser(Guid matchId, CancellationToken cancellationToken);
    Task<List<MatchApplication>> GetPendingByUserId(Guid userId, CancellationToken cancellationToken);
    void Insert(MatchApplication matchApplication);
    void Remove(MatchApplication matchApplication);

}