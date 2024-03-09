namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetPhoneBookUsersInput : IRequest<GetPhoneBookUsersResponse>
{
    public List<string> PhoneNumbers { get; set; }
}