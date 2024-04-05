using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Events;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.DomainEvents;

public class UpdateAnnouncementOnMatchApplicationAccepted : IDomainEventHandler<MatchApplicationAcceptedDomainEvent>
{
    private readonly IMatchRepository _matchRepository;

    public UpdateAnnouncementOnMatchApplicationAccepted(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task Handle(MatchApplicationAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var maybeAnnouncement = await _matchRepository.GetAnnouncementByMatchIdAsync(domainEvent.MatchApplication.MatchId, cancellationToken);

        if (maybeAnnouncement.HasNoValue)
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.NotAnnounced);
        }

        var announcement = maybeAnnouncement.Value;

        announcement.NumberOfPlayersAccepted++;

        if (announcement.NumberOfPlayersAccepted > announcement.NumberOfPlayersLimit)
        {
            throw new DomainException(DomainErrors.MatchApplication.PlayersLimitReached);
        }

        if (announcement.NumberOfPlayersAccepted == announcement.NumberOfPlayersLimit)
        {
            announcement.Delete();
        }
    }
}