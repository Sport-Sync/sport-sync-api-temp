using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.FriendshipRequests.GetPendingFriendshipRequests;

public class GetPendingFriendshipRequestsHandler : IRequestHandler<List<FriendshipRequestType>>
{
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetPendingFriendshipRequestsHandler(
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserIdentifierProvider userIdentifierProvider)
    {
        _friendshipRequestRepository = friendshipRequestRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<List<FriendshipRequestType>> Handle(CancellationToken cancellationToken)
    {
        var pendingFriendshipRequests = await _friendshipRequestRepository.GetPendingForFriendIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        var friendshipRequestTypes = pendingFriendshipRequests.Select(FriendshipRequestType.FromFriendshipRequest);
        
        return friendshipRequestTypes.ToList();
    }
}