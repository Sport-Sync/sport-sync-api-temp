using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetPhoneBookUsersResponse
{
    public List<ExtendedUserType> Users { get; set; } = new ();
}