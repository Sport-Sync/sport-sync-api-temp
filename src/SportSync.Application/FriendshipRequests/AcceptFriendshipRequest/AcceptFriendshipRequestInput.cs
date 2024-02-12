using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.FriendshipRequests.AcceptFriendshipRequest;

public class AcceptFriendshipRequestInput : IRequest<Result>
{
    public Guid FriendshipRequestId { get; set; }
}