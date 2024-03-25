using System.Linq.Expressions;
using SportSync.Domain.Entities;
using SportSync.Domain.Types.Abstraction;

namespace SportSync.Domain.Types;

public class UserType
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public UserType(User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
    }

    public UserType()
    {

    }

    public static Expression<Func<User, UserType>> PropertySelector = x => new UserType
    {
        Id = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone.Value
    };
}

public class UserProfileType : UserType, IPendingFriendshipRequestsInfo
{
    public string ImageUrl { get; set; }
    public List<UserProfileType> MutualFriends { get; set; }
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }
    public bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;

    public UserProfileType(User user, PendingFriendshipRequestType pendingFriendshipRequest = null, List<UserProfileType> mutualFriends = null, string imageUrl = null)
        : base(user)
    {

        PendingFriendshipRequest = pendingFriendshipRequest;
        MutualFriends = mutualFriends;
        ImageUrl = imageUrl;
    }

    public UserProfileType(User user, string imageUrl = null)
        : base(user)
    {
        ImageUrl = imageUrl;
    }
}

public class PhoneBookUserType : UserType, IPendingFriendshipRequestsInfo
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
}