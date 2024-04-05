using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.DomainEvents;

public class AddPlayersOnEventMembersAddedHandler : IDomainEventHandler<EventMembersAddedDomainEvent>
{
    private readonly IMatchRepository _matchRepository;

    public AddPlayersOnEventMembersAddedHandler(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task Handle(EventMembersAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var matches = await _matchRepository.GetByEventId(domainEvent.Event.Id, cancellationToken);

        var futureMatches = matches.Where(m => !m.HasPassed());

        foreach (var match in futureMatches)
        {
            match.AddPlayers(domainEvent.UserIds);
        }
    }
}