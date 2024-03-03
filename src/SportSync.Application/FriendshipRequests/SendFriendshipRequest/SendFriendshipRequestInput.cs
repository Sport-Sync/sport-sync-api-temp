using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.FriendshipRequests.SendFriendshipRequest;

public class SendFriendshipRequestInput : IRequest<Result>
{
    public List<Guid> FriendIds { get; set; }
}