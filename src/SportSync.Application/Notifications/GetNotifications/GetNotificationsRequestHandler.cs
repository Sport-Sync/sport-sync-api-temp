using Newtonsoft.Json;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.GetNotifications;

public class GetNotificationsRequestHandler : IRequestHandler<GetNotificationsInput, GetNotificationsResponse>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetNotificationsRequestHandler(INotificationRepository notificationRepository, IUserIdentifierProvider userIdentifierProvider)
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

        var notifications = _notificationRepository
            .GetQueryable(x => x.UserId == _userIdentifierProvider.UserId && x.CompletedOnUtc == null)
            .Take(request.Count)
            .ToList();

        return new GetNotificationsResponse { Notifications = notifications };
    }
}