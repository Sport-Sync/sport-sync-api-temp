using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Repositories;
using SportSync.Domain.Services.Factories.Notification;
using SportSync.Domain.Types;

namespace SportSync.Application.Notifications.GetNotifications;

public class GetNotificationsRequestHandler : IRequestHandler<GetNotificationsInput, GetNotificationsResponse>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetNotificationsRequestHandler(
        INotificationRepository notificationRepository,
        IUserIdentifierProvider userIdentifierProvider)
    {
        _notificationRepository = notificationRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<GetNotificationsResponse> Handle(GetNotificationsInput request, CancellationToken cancellationToken)
    {
        if (request.Count <= 0)
        {
            return new GetNotificationsResponse();
        }

        var notifications = await _notificationRepository
            .Where(x => x.UserId == _userIdentifierProvider.UserId)
            .Take(request.Count)
            .Select(NotificationType.PropertySelector)
            .ToListAsync(cancellationToken);

        var contentProvider = NotificationContentFactory.GetContentProvider(request.Language);

        foreach (var notification in notifications)
        {
            notification.Content = contentProvider.Content(notification.Type, notification.ContentData);
        }

        return new GetNotificationsResponse { Notifications = notifications };
    }
}