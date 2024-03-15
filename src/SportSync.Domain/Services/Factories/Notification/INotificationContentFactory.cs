using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Services.Factories.Notification;

public interface INotificationContentFactory
{
    string Content(NotificationTypeEnum type, NotificationContentData contentData);
}