using System.Linq.Expressions;
using HotChocolate;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class UserType
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ImageUrl { get; set; }

    [GraphQLIgnore]
    public bool HasProfileImage { get; set; }

    public UserType(User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
        HasProfileImage = user.HasProfileImage;
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
        Phone = x.Phone.Value,
        HasProfileImage = x.HasProfileImage
    };
}