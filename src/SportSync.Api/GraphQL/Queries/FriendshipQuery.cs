using HotChocolate.Authorization;
using SportSync.Application.Core.Common;
using SportSync.Application.Users.GetFriends;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class FriendshipQuery
{

    [Authorize]
    public async Task<PagedList<UserType>> GetFriends(
        [Service] GetFriendsRequestHandler requestHandler,
        GetFriendsInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}