using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class NotificationQuery
{
    [Authorize]
    public IQueryable<NotificationType> GetNotifications(
        [Service] INotificationRepository repository,
        [Service] IUserIdentifierProvider userIdProvider) => 
            repository.GetQueryable(x => x.UserId == userIdProvider.UserId && x.CompletedOnUtc == null);
}