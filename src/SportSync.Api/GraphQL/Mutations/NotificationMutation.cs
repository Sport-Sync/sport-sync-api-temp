using HotChocolate.Authorization;
using SportSync.Application.Notifications.DeleteNotification;
using SportSync.Domain.Core.Primitives.Result;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class NotificationMutation
{
    [Authorize]
    public async Task<Result> DeleteNotification(
        [Service] DeleteNotificationRequestHandler requestHandler,
        DeleteNotificationInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}