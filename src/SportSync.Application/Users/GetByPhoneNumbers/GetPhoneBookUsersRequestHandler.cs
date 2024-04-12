using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Users.GetByPhoneNumbers;

public class GetPhoneBookUsersRequestHandler : IRequestHandler<GetPhoneBookUsersInput, GetPhoneBookUsersResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetPhoneBookUsersRequestHandler(
        IUserRepository userRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IFriendshipRequestRepository friendshipRequestRepository)
    {
        _userRepository = userRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _friendshipRequestRepository = friendshipRequestRepository;
    }

    public async Task<GetPhoneBookUsersResponse> Handle(GetPhoneBookUsersInput request, CancellationToken cancellationToken)
    {
        if (!request.PhoneNumbers.Any())
        {
            return new GetPhoneBookUsersResponse();
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
            .Where(user => 
                phoneNumbers.Contains(user.Phone.Value) && 
                user.Id != currentUserId &&
                user.FriendInvitees.All(fi => fi.UserId != currentUserId) &&
                user.FriendInviters.All(inv => inv.FriendId != currentUserId))
            .ToListAsync(cancellationToken);

        var friendshipRequests = await _friendshipRequestRepository.GetAllPendingForUserIdAsync(currentUserId, cancellationToken);

        var phoneBookUsers = new List<ExtendedUserType>();
        
        foreach (var user in users)
        {
            var isFriendWithCurrentUser = user.IsFriendWith(currentUserId);
            var pendingFriendshipRequest = friendshipRequests.FirstOrDefault(x => x.FriendId == user.Id || x.UserId == user.Id);
            
            phoneBookUsers.Add(new ExtendedUserType(user, isFriendWithCurrentUser, pendingFriendshipRequest));
        }

        return new GetPhoneBookUsersResponse { Users = phoneBookUsers };
    }
}