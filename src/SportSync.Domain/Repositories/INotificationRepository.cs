using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface INotificationRepository
{
    void Insert(Notification notification);
}