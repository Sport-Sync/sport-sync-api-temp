using Microsoft.Extensions.Logging;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Events;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Termins.AcceptTerminApplication;

public class AddPlayerOnTerminApplicationAcceptedDomainEventHandler : IDomainEventHandler<TerminApplicationAcceptedDomainEvent>
{
    private readonly ITerminRepository _terminRepository;
    private readonly ILogger<AddPlayerOnTerminApplicationAcceptedDomainEventHandler> _logger;

    public AddPlayerOnTerminApplicationAcceptedDomainEventHandler(
        ITerminRepository terminRepository,
        ILogger<AddPlayerOnTerminApplicationAcceptedDomainEventHandler> logger)
    {
        _terminRepository = terminRepository;
        _logger = logger;
    }


    public async Task Handle(TerminApplicationAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var maybeTermin = await _terminRepository.GetByIdAsync(domainEvent.TerminApplication.TerminId, cancellationToken);
        if (maybeTermin.HasNoValue)
        {
            _logger.LogError("Termin with id {terminId} not found so player could not be added after application {terminApplicationId} accepted.",
                domainEvent.TerminApplication.TerminId,
                domainEvent.TerminApplication.Id);

            throw new DomainException(DomainErrors.Termin.NotFound);
        }

        var termin = maybeTermin.Value;
        termin.AddPlayer(domainEvent.TerminApplication.AppliedByUserId);
    }
}