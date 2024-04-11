using System.Linq.Expressions;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class UserProfileType : UserType
{
    public FriendshipInformationType FriendshipInformation { get; set; }

    public UserProfileType(User user, FriendshipInformationType friendshipInformation) : base(user)
    {
        FriendshipInformation = friendshipInformation;
    }

    public UserProfileType(User user) : base(user)
    {
    }

    public UserProfileType()
    {
        
    }

    public static new Expression<Func<User, UserProfileType>> PropertySelector = x => new UserProfileType
    {
        Id = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone.Value,
        HasProfileImage = x.HasProfileImage
    };
}