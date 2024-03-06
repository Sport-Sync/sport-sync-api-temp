namespace SportSync.Application.Notifications.GetNotifications;

public class GetNotificationsInput : IRequest<GetNotificationsResponse>
{
    public int Count { get; set; }
}