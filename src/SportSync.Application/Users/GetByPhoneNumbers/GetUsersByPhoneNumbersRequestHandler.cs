using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetUsersByPhoneNumbersRequestHandler : IRequestHandler<GetUsersByPhoneNumbersInput, GetUsersByPhoneNumbersResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetUsersByPhoneNumbersRequestHandler(
        IUserRepository userRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IFriendshipRequestRepository friendshipRequestRepository)
    {
        _userRepository = userRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _friendshipRequestRepository = friendshipRequestRepository;
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

        var currentUserId = _userIdentifierProvider.UserId;

        var users = await _userRepository
            .GetQueryable(user => 
                phoneNumbers.Contains(user.Phone.Value) && 
                user.Id != currentUserId &&
                user.FriendInvitees.All(fi => fi.UserId != currentUserId) &&
                user.FriendInviters.All(inv => inv.FriendId != currentUserId))
            .ToListAsync(cancellationToken);

        var friendshipRequests = _friendshipRequestRepository.GetAllPendingForUserIdAsync(currentUserId);

        // TODO: set pending friendshipRequests to response as a flag

        return new GetUsersByPhoneNumbersResponse { Users = users };
    }
}