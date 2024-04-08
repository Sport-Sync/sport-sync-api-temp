using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class FriendshipRequestType
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
    public bool Accepted { get; set; }
    public bool Rejected { get; set; }
    public UserType Sender { get; set; }

    public static FriendshipRequestType FromFriendshipRequest(FriendshipRequest friendshipRequest) => new()
    {
        Id = friendshipRequest.Id,
        Accepted = friendshipRequest.Accepted,
        Rejected = friendshipRequest.Rejected,
        UserId = friendshipRequest.UserId,
        FriendId = friendshipRequest.FriendId,
        Sender = new UserType(friendshipRequest.User)
    };
}