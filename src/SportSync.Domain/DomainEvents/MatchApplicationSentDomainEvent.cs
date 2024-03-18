using SportSync.Domain.Core.Events;
using SportSync.Domain.Entities;

namespace SportSync.Domain.DomainEvents;

public record MatchApplicationSentDomainEvent(MatchApplication MatchApplication, Match Match) : IDomainEvent;