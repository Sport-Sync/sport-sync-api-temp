using SportSync.Domain.Types;

namespace SportSync.Application.Notifications.GetNotifications;

public class GetNotificationsResponse
{
    public List<NotificationType> Notifications { get; set; } = new ();
}