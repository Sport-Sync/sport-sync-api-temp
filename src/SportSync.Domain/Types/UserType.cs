using System.Linq.Expressions;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public interface IPendingFriendshipRequestsInfo
{
    PendingFriendshipRequestType PendingFriendshipRequest { get; set; }
    bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;
}

public class BaseUserType
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public static Expression<Func<User, BaseUserType>> PropertySelector = x => new BaseUserType
    {
        Id = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone.Value
    };

    public static BaseUserType FromUser(User user) => PropertySelector.Compile()(user);
}

public class UserType : BaseUserType, IPendingFriendshipRequestsInfo
{
    public string ImageUrl { get; set; }
    public FriendType MutualFriends { get; set; }
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }

    public UserType(BaseUserType user, PendingFriendshipRequestType pendingFriendshipRequest = null, FriendType mutualFriends = null, string imageUrl = null)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
        PendingFriendshipRequest = pendingFriendshipRequest;
        MutualFriends = mutualFriends;
        ImageUrl = imageUrl;
    }
}

public class PhoneBookUserType : BaseUserType, IPendingFriendshipRequestsInfo
{
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }

    public PhoneBookUserType(BaseUserType user, PendingFriendshipRequestType pendingFriendshipRequest = null)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
        PendingFriendshipRequest = pendingFriendshipRequest;
    }
}

public class PendingFriendshipRequestType
{
    public Guid FriendshipRequestId { get; set; }
    public bool SentByMe { get; set; }

    public static PendingFriendshipRequestType Create(Guid friendshipRequestId, bool sentByMe) =>
        new()
        {
            FriendshipRequestId = friendshipRequestId,
            SentByMe = sentByMe
        };
}

public class FriendType : BaseUserType
{
    public string ImageUrl { get; set; }

    public FriendType(User user, string imageUrl = null) : base(user)
    {
        ImageUrl = imageUrl;
    }
}