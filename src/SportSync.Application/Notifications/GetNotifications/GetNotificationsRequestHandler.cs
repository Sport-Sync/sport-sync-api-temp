﻿using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Repositories;
using SportSync.Domain.Services.Factories.Notification;

namespace SportSync.Application.Notifications.GetNotifications;

public class GetNotificationsRequestHandler : IRequestHandler<GetNotificationsInput, GetNotificationsResponse>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IHttpHeaderProvider _httpHeaderProvider;

    public GetNotificationsRequestHandler(
        INotificationRepository notificationRepository, 
        IUserIdentifierProvider userIdentifierProvider, 
        IHttpHeaderProvider httpHeaderProvider)
    {
        _notificationRepository = notificationRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _httpHeaderProvider = httpHeaderProvider;
    }

    public async Task<GetNotificationsResponse> Handle(GetNotificationsInput request, CancellationToken cancellationToken)
    {
        if (request.Count <= 0)
        {
            return new GetNotificationsResponse();
        }

        var notifications = await _notificationRepository
            .GetQueryable(x => x.UserId == _userIdentifierProvider.UserId)
            .Take(request.Count)
            .ToListAsync(cancellationToken);

        var maybeLanguage = _httpHeaderProvider.Get("language");
        var language = maybeLanguage.HasValue ? maybeLanguage.Value : "Hr";

        var contentFactory = NotificationLocalizedFactory.GetLocalizedFactory(language);

        foreach (var notification in notifications)
        {
            notification.Content = contentFactory.Content(notification.Type, notification.ContentData);
        }

        return new GetNotificationsResponse { Notifications = notifications };
    }
}