using System.Linq.Expressions;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class UserType
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public static Expression<Func<User, UserType>> PropertySelector = x => new UserType
    {
        Id = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone.Value
    };

    public static UserType FromUser(User user) => PropertySelector.Compile()(user);
}

public class PhoneBookUserType : UserType
{
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }

    public bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;


    public PhoneBookUserType(UserType user, PendingFriendshipRequestType pendingFriendshipRequest = null)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
        PendingFriendshipRequest = pendingFriendshipRequest;
    }

    public PhoneBookUserType()
    {

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