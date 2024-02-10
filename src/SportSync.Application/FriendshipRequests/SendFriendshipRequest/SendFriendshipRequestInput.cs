using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.FriendshipRequests.SendFriendshipRequest;

public class SendFriendshipRequestInput : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
}