using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record FriendAddedToMatchDomainEvent(User UserInitiator, Guid FriendId, Match Match) : IDomainEvent;