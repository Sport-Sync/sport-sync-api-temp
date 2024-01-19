using SportSync.Domain.Entities;
using System.Linq.Expressions;

namespace SportSync.Domain.DtoTypes;

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
        Phone = x.Phone
    };
}