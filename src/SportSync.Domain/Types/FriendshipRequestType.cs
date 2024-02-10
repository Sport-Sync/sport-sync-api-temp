using System.Linq.Expressions;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class FriendshipRequestType
{
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
    public bool Accepted { get; set; }
    public bool Rejected { get; set; }

    public static Expression<Func<FriendshipRequest, FriendshipRequestType>> PropertySelector = x => new FriendshipRequestType
    {
        Accepted = x.Accepted,
        Rejected = x.Rejected,
        UserId = x.UserId,
        FriendId = x.FriendId
    };

    public static FriendshipRequestType FromFriendshipRequest(FriendshipRequest friendshipRequest)
    {
        return PropertySelector.Compile()(friendshipRequest);
    }
}