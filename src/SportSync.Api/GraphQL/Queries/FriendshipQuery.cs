using HotChocolate.Authorization;
using SportSync.Application.FriendshipRequests.GetPendingFriendshipRequests;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class FriendshipQuery
{
    [Authorize]
    public async Task<List<FriendshipRequestType>> GetPendingFriendshipRequests(
        [Service] GetPendingFriendshipRequestsHandler requestHandler,
        CancellationToken cancellationToken) => await requestHandler.Handle(cancellationToken);
}