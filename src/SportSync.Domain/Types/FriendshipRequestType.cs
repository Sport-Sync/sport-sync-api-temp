using System.Linq.Expressions;
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

    public static Expression<Func<FriendshipRequest, FriendshipRequestType>> PropertySelector = x => new FriendshipRequestType
    {
        Id = x.Id,
        Accepted = x.Accepted,
        Rejected = x.Rejected,
        UserId = x.UserId,
        FriendId = x.FriendId,
        Sender = new UserType(x.User)
    };
}