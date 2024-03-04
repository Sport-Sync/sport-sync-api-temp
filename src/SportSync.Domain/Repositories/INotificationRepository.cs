using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Types;

namespace SportSync.Domain.Repositories;

public interface INotificationRepository : IQueryableRepository<Notification, NotificationType>
{
    Task<Maybe<Notification>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Notification>> GetByResourceIdAndType(Guid resourceId, NotificationTypeEnum type, CancellationToken cancellationToken);
    void Insert(Notification notification);
    void Remove(Notification notification);
}