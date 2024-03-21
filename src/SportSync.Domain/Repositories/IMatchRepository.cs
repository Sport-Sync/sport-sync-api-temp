using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Domain.Repositories;

public interface IMatchRepository : IQueryableRepository<Match, MatchType>
{
    Task<Maybe<Match>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Match>> GetByEventId(Guid eventId, CancellationToken cancellationToken);
    Task<List<Player>> GetPlayers(Guid id, CancellationToken cancellationToken);
    Task<List<EventMember>> GetAdmins(Guid matchId, CancellationToken cancellationToken);
    Task<List<(Match LastMatch, int PendingMatchesCount)>> GetLastRepeatableMatches();
    Task<List<Match>> GetAnnouncedMatches(DateTime date, CancellationToken cancellationToken);
    void InsertRange(IReadOnlyCollection<Match> matches);
}