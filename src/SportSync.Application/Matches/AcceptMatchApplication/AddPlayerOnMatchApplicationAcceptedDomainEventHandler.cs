using Microsoft.Extensions.Logging;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Events;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.AcceptMatchApplication;

public class AddPlayerOnMatchApplicationAcceptedDomainEventHandler : IDomainEventHandler<MatchApplicationAcceptedDomainEvent>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ILogger<AddPlayerOnMatchApplicationAcceptedDomainEventHandler> _logger;

    public AddPlayerOnMatchApplicationAcceptedDomainEventHandler(
        IMatchRepository matchRepository,
        ILogger<AddPlayerOnMatchApplicationAcceptedDomainEventHandler> logger)
    {
        _matchRepository = matchRepository;
        _logger = logger;
    }


    public async Task Handle(MatchApplicationAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var maybeMatch = await _matchRepository.GetByIdAsync(domainEvent.MatchApplication.MatchId, cancellationToken);
        if (maybeMatch.HasNoValue)
        {
            _logger.LogError("Match with id {matchId} not found so player could not be added after application {matchApplicationId} accepted.",
                domainEvent.MatchApplication.MatchId,
                domainEvent.MatchApplication.Id);

            throw new DomainException(DomainErrors.Match.NotFound);
        }

        var match = maybeMatch.Value;
        match.AddPlayer(domainEvent.MatchApplication.AppliedByUserId);
    }
}