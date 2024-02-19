using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetUsersByPhoneNumbersResponse
{
    public List<UserType> Users { get; set; } = new ();
}