using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetPhoneBookUsersResponse
{
    public List<PhoneBookUserType> Users { get; set; } = new ();
}