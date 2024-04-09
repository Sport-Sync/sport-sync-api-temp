using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.FriendshipRequests.GetPendingFriendshipRequests;

public class GetPendingFriendshipRequestsHandler : IRequestHandler<List<FriendshipRequestType>>
{
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IBlobStorageService _blobStorageService;

    public GetPendingFriendshipRequestsHandler(
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IBlobStorageService blobStorageService)
    {
        _friendshipRequestRepository = friendshipRequestRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _blobStorageService = blobStorageService;
    }

    public async Task<List<FriendshipRequestType>> Handle(CancellationToken cancellationToken)
    {
        var pendingFriendshipRequests = await _friendshipRequestRepository.GetPendingForFriendIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        var friendshipRequestTypes = pendingFriendshipRequests.Select(FriendshipRequestType.FromFriendshipRequest).ToList();

        foreach (var friendshipRequest in friendshipRequestTypes.Where(x => x.Sender.HasProfileImage))
        {
            friendshipRequest.Sender.ImageUrl = await _blobStorageService.GetProfileImageUrl(friendshipRequest.UserId);
        }

        return friendshipRequestTypes;
    }
}