using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record MatchApplicationAcceptedDomainEvent(MatchApplication MatchApplication) : IDomainEvent;