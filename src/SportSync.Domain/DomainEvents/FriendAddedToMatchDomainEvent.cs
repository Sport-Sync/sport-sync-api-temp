using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record FriendAddedToMatchDomainEvent(User AddedByUser, Guid FriendId, Match Match) : IDomainEvent;