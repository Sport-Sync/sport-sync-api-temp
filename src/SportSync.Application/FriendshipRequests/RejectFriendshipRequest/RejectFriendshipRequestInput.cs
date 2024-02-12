using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.FriendshipRequests.RejectFriendshipRequest;

public class RejectFriendshipRequestInput : IRequest<Result>
{
    public Guid FriendshipRequestId { get; set; }
}