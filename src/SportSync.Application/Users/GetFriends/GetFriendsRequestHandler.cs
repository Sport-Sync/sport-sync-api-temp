using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Common;
using SportSync.Application.Core.Constants;
using SportSync.Application.Core.Services;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetFriends;

public class GetFriendsRequestHandler : IRequestHandler<GetFriendsInput, PagedList<ExtendedUserType>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileImageService _userProfileImageService;

    public GetFriendsRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IFriendshipRequestRepository friendshipRequestRepository,
        IUserRepository userRepository,
        IUserProfileImageService userProfileImageService)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _friendshipRequestRepository = friendshipRequestRepository;
        _userRepository = userRepository;
        _userProfileImageService = userProfileImageService;
    }

    public async Task<PagedList<ExtendedUserType>> Handle(GetFriendsInput request, CancellationToken cancellationToken)
    {
        var currentUserId = _userIdentifierProvider.UserId;

        var friends = _userRepository.Where(
            u => (u.FriendInvitees.Any(x => x.UserId == currentUserId) || u.FriendInviters.Any(x => x.FriendId == currentUserId)) &&
                 (request.Search == null || u.FirstName.StartsWith(request.Search) || u.LastName.StartsWith(request.Search)))
            .OrderBy(u => u.FirstName);

        var totalCount = await friends.CountAsync(cancellationToken);

        var firstPageSize = request.FirstPageSize ?? PaginationConstants.FirstPageSize;

        var skip = request.Page == 1 ? 0 :
            firstPageSize + ((request.Page - 2) * request.PageSize);

        var friendsPage = await friends
            .Skip(skip)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken);

        var currentUserFriendshipRequests = await _friendshipRequestRepository.GetAllPendingForUserIdAsync(currentUserId, cancellationToken);

        var friendshipRequestInfoResult = new List<ExtendedUserType>();

        foreach (var friend in friendsPage)
        {
            var isFriendWithCurrentUser = friend.IsFriendWith(currentUserId);
            var pendingFriendshipRequest = currentUserFriendshipRequests.FirstOrDefault(x => x.FriendId == friend.Id || x.UserId == friend.Id);

            friendshipRequestInfoResult.Add(new ExtendedUserType(friend, isFriendWithCurrentUser, pendingFriendshipRequest));
        }

        await _userProfileImageService.PopulateImageUrl(friendshipRequestInfoResult.ToArray());


        return new PagedList<ExtendedUserType>(friendshipRequestInfoResult, request.Page, request.PageSize, totalCount, firstPageSize);
    }
}