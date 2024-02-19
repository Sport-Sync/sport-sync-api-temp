namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetUsersByPhoneNumbersInput : IRequest<GetUsersByPhoneNumbersResponse>
{
    public List<string> PhoneNumbers { get; set; }
}