using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record EventInvitationRejectedDomainEvent(EventInvitation Invitation, User User, Event Event) : IDomainEvent;