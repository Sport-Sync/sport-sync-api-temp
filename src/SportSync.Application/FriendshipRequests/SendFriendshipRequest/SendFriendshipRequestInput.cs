using SportSync.Application.Core.Abstractions.Common;
using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.FriendshipRequests.SendFriendshipRequest;

public class SendFriendshipRequestInput : IRequest<Result>
{
    public Guid UserId { get; }
    public Guid FriendId { get; }
}