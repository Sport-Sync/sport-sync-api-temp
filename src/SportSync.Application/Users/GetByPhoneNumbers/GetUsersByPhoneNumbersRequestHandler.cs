using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetUsersByPhoneNumbersRequestHandler : IRequestHandler<GetUsersByPhoneNumbersInput, GetUsersByPhoneNumbersResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetUsersByPhoneNumbersRequestHandler(IUserRepository userRepository, IUserIdentifierProvider userIdentifierProvider)
    {
        _userRepository = userRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<GetUsersByPhoneNumbersResponse> Handle(GetUsersByPhoneNumbersInput request, CancellationToken cancellationToken)
    {
        if (!request.PhoneNumbers.Any())
        {
            return new GetUsersByPhoneNumbersResponse();
        }

        var phoneNumbersResult = request.PhoneNumbers.Select(PhoneNumber.Create).ToList();

        var validPhoneNumbers = phoneNumbersResult.Where(p => p.IsSuccess).ToList();

        if (!validPhoneNumbers.Any())
        {
            throw new DomainException(phoneNumbersResult.First().Error);
        }

        var phoneNumbers = validPhoneNumbers.Select(p => p.Value.Value).ToList();

        var users = await _userRepository
            .GetQueryable(user => phoneNumbers.Contains(user.Phone.Value) && user.Id != _userIdentifierProvider.UserId)
            .ToListAsync(cancellationToken);

        return new GetUsersByPhoneNumbersResponse { Users = users };
    }
}