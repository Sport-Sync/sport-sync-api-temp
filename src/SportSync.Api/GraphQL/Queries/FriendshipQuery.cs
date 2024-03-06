using System.Linq.Expressions;
using HotChocolate.Authorization;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Common;
using SportSync.Application.Users.GetFriends;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
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

    [Authorize]
    [UseProjection]
    public IQueryable<FriendshipRequestType> GetPendingFriendshipRequests(
        [Service] IFriendshipRequestRepository _repository,
        [Service] IUserIdentifierProvider _userIdentifierProvider) => 
            _repository.GetQueryable(x => x.FriendId == _userIdentifierProvider.UserId &&
                                          x.CompletedOnUtc == null);
}