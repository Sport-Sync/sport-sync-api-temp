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

    public UserType(User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
        ImageUrl = user.ImageUrl;
    }

    public UserType()
    {

    }
}