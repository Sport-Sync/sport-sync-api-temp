using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IEventRepository
{
    Task<Maybe<Event>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Maybe<Event>> GetByEventInvitationIdAsync(Guid eventInvitationId, CancellationToken cancellationToken);
    Task<List<EventInvitation>> GetPendingInvitations(Guid eventId, CancellationToken cancellationToken);
    void Insert(Event @event);
    Task EnsureUserIsAdminOnEvent(Guid eventId, Guid userId, CancellationToken cancellationToken);
    Task<bool> IsAdminOnEvent(Guid eventId, Guid userId, CancellationToken cancellationToken);
}
