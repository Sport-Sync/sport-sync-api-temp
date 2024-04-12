using HotChocolate.Authorization;
using SportSync.Application.Core.Common;
using SportSync.Application.FriendshipRequests.GetPendingFriendshipRequests;
using SportSync.Application.Users.GetFriends;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class FriendshipQuery
{
    [Authorize]
    public async Task<PagedList<ExtendedUserType>> GetFriends(
        [Service] GetFriendsRequestHandler requestHandler,
        GetFriendsInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<List<FriendshipRequestType>> GetPendingFriendshipRequests(
        [Service] GetPendingFriendshipRequestsHandler requestHandler,
        CancellationToken cancellationToken) => await requestHandler.Handle(cancellationToken);
}