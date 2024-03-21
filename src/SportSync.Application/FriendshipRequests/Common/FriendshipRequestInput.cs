using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.FriendshipRequests.Common;

public class FriendshipRequestInput : IRequest<Result>
{
    public Guid FriendshipRequestId { get; set; }
}