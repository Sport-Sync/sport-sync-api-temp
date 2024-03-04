using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Notifications.DeleteNotification;

public class DeleteNotificationInput : IRequest<Result>
{
    public Guid NotificationId { get; set; }
}