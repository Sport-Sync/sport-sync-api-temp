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
    public bool PendingFriendshipRequest { get; set; }

    public PhoneBookUserType(UserType user, bool pendingFriendshipRequest)
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