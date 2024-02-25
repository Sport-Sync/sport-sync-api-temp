using Microsoft.EntityFrameworkCore;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetUsersByPhoneNumbersRequestHandler : IRequestHandler<GetUsersByPhoneNumbersInput, GetUsersByPhoneNumbersResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUsersByPhoneNumbersRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUsersByPhoneNumbersResponse> Handle(GetUsersByPhoneNumbersInput request, CancellationToken cancellationToken)
    {
        if (!request.PhoneNumbers.Any())
        {
            return new GetUsersByPhoneNumbersResponse();
        }

        var formattedPhoneNumbers = request.PhoneNumbers.Select(PhoneNumber.Format).ToList();
        var phoneNumbersResult = formattedPhoneNumbers.Select(PhoneNumber.Create).ToList();

        var validPhoneNumbers = phoneNumbersResult.Where(p => p.IsSuccess).ToList();

        if (!validPhoneNumbers.Any())
        {
            throw new DomainException(phoneNumbersResult.First().Error);
        }

        var phoneNumbers = validPhoneNumbers.Select(p => p.Value.Value).ToList();

        var users = await _userRepository
            .GetQueryable(user => phoneNumbers.Contains(user.Phone.Value))
            .ToListAsync(cancellationToken);

        return new GetUsersByPhoneNumbersResponse { Users = users };
    }
}