using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.DomainEvents;

public class AddPlayersOnEventMembersAddedHandler : IDomainEventHandler<EventMembersAddedDomainEvent>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IDateTime _dateTime;

    public AddPlayersOnEventMembersAddedHandler(IMatchRepository matchRepository, IDateTime dateTime)
    {
        _matchRepository = matchRepository;
        _dateTime = dateTime;
    }

    public async Task Handle(EventMembersAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var matches = await _matchRepository.GetByEventId(domainEvent.Event.Id, cancellationToken);

        var futureMatches = matches.Where(m => !m.HasPassed(_dateTime.UtcNow));

        foreach (var match in futureMatches)
        {
            match.AddPlayers(domainEvent.UserIds);
        }
    }
}