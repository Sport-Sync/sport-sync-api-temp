using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record EventInvitationSentDomainEvent(EventInvitation EventInvitation, Event Event) : IDomainEvent;