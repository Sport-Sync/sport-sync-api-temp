using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record EventInvitationAcceptedDomainEvent(EventInvitation Invitation, User User, Event Event) : IDomainEvent;