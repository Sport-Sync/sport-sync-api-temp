using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record EventMembersAddedDomainEvent(Event Event, List<Guid> UserIds) : IDomainEvent;