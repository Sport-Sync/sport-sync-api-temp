using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public sealed class EventCreatedDomainEvent : IDomainEvent
{
    internal EventCreatedDomainEvent(Event createdEvent) => CreatedEvent = createdEvent;

    /// <summary>
    /// Gets the personal event.
    /// </summary>
    public Event CreatedEvent { get; }
}