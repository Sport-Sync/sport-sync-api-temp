using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Notifications.CompleteNotification;

public class CompleteNotificationInput : IRequest<Result>
{
    public Guid NotificationId { get; set; }
}