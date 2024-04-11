using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Services;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.FriendshipRequests.GetPendingFriendshipRequests;

public class GetPendingFriendshipRequestsHandler : IRequestHandler<List<FriendshipRequestType>>
{
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserProfileImageService _userProfileImageService;

    public GetPendingFriendshipRequestsHandler(
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IUserProfileImageService userProfileImageService)
    {
        _friendshipRequestRepository = friendshipRequestRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _userProfileImageService = userProfileImageService;
    }

    public async Task<List<FriendshipRequestType>> Handle(CancellationToken cancellationToken)
    {
        var pendingFriendshipRequests = await _friendshipRequestRepository.GetPendingForFriendIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        var friendshipRequestTypes = pendingFriendshipRequests.Select(FriendshipRequestType.FromFriendshipRequest);

        await _userProfileImageService.PopulateImageUrl(friendshipRequestTypes.ToArray());

        return friendshipRequestTypes.ToList();
    }
}