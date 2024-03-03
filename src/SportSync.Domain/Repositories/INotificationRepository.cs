using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetByResourceIdAndType(Guid resourceId, NotificationType type, CancellationToken cancellationToken);

    void Insert(Notification notification);
}