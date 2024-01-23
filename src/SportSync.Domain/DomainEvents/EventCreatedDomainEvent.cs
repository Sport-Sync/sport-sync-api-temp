using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record EventCreatedDomainEvent(Event CreatedEvent) : IDomainEvent;