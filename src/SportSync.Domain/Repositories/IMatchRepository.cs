using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IMatchRepository
{
    Task<Maybe<Match>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Maybe<MatchAnnouncement>> GetAnnouncementByMatchIdAsync(Guid matchId, CancellationToken cancellationToken);
    Task<List<Match>> GetByEventId(Guid eventId, CancellationToken cancellationToken);
    Task<List<Player>> GetPlayers(Guid id, CancellationToken cancellationToken);
    Task<List<EventMember>> GetAdmins(Guid matchId, CancellationToken cancellationToken);
    Task<List<(Match LastMatch, int PendingMatchesCount)>> GetLastRepeatableMatches();
    Task<List<Match>> GetAnnouncedMatches(DateTime date, CancellationToken cancellationToken);
    Task<List<Match>> GetUserMatchesOnDate(DateTime date, Guid userId, CancellationToken cancellationToken);
    Task<List<Match>> GetFutureUserMatches(Guid userId, CancellationToken cancellationToken);
    void InsertRange(IReadOnlyCollection<Match> matches);
}