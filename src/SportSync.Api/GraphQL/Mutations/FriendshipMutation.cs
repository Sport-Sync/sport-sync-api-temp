using HotChocolate.Authorization;
using SportSync.Application.FriendshipRequests.AcceptFriendshipRequest;
using SportSync.Application.FriendshipRequests.CancelFriendshipRequest;
using SportSync.Application.FriendshipRequests.Common;
using SportSync.Application.FriendshipRequests.RejectFriendshipRequest;
using SportSync.Application.FriendshipRequests.SendFriendshipRequest;
using SportSync.Application.Users.RemoveFriendship;
using SportSync.Domain.Core.Primitives.Result;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class FriendshipMutation
{
    [Authorize]
    public async Task<Result> SendFriendshipRequest(
        [Service] SendFriendshipRequestHandler requestHandler,
        SendFriendshipRequestInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<Result> AcceptFriendshipRequest(
        [Service] AcceptFriendshipRequestHandler requestHandler,
        FriendshipRequestInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<Result> RejectFriendshipRequest(
        [Service] RejectFriendshipRequestHandler requestHandler,
        FriendshipRequestInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<Result> CancelFriendshipRequest(
        [Service] CancelFriendshipRequestHandler requestHandler,
        FriendshipRequestInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);


    [Authorize]
    public async Task<Result> RemoveFriendship(
        [Service] RemoveFriendshipRequestHandler requestHandler,
        RemoveFriendshipInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}