using HotChocolate.Authorization;
using SportSync.Application.Notifications.GetNotifications;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class NotificationQuery
{
    [Authorize]
    public async Task<GetNotificationsResponse> GetNotifications(
        [Service] GetNotificationsRequestHandler requestHandler,
        GetNotificationsInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}