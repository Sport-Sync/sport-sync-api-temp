using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.FriendshipRequests.CancelFriendshipRequest;

public class CancelFriendshipRequestInput : IRequest<Result>
{
    public Guid FriendshipRequestId { get; set; }
}