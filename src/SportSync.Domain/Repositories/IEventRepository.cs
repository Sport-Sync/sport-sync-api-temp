using SportSync.Domain.Entities;
namespace SportSync.Domain.Repositories;

public interface IEventRepository
{
    void Insert(Event @event);
    Task EnsureUserIsAdminOnEvent(Guid eventId, Guid userId, CancellationToken cancellationToken);
}
