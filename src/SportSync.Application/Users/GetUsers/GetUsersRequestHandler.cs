using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Common;
using SportSync.Application.Core.Constants;
using SportSync.Application.Core.Services;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Services;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUsers;

public class GetUsersRequestHandler : IRequestHandler<GetUsersInput, PagedList<ExtendedUserType>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserProfileImageService _userProfileImageService;

    public GetUsersRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IUserProfileImageService userProfileImageService,
        IFriendshipRequestRepository friendshipRequestRepository)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _userProfileImageService = userProfileImageService;
        _friendshipRequestRepository = friendshipRequestRepository;
    }

    public async Task<PagedList<ExtendedUserType>> Handle(GetUsersInput request, CancellationToken cancellationToken)
    {
        var currentUserId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var currentUser = maybeUser.Value;

        var usersQuery = _userRepository.Where(u =>
                u.Id != currentUserId &&
                (request.Search == null || u.FirstName.StartsWith(request.Search) || u.LastName.StartsWith(request.Search)))
            .OrderBy(u => u.FirstName);

        var totalCount = await usersQuery.CountAsync(cancellationToken);

        var firstPageSize = request.FirstPageSize ?? PaginationConstants.FirstPageSize;

        var skip = request.Page == 1 ? 0 :
            firstPageSize + ((request.Page - 2) * request.PageSize);

        var usersPage = await usersQuery
            .Skip(skip)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken);

        var usersResult = new List<ExtendedUserType>();

        var friendshipRequests = await _friendshipRequestRepository.GetAllPendingForUserIdAsync(currentUserId, cancellationToken);
        var currentUserFriends = await _userRepository.GetByIdsAsync(currentUser.Friends.ToList(), cancellationToken);

        foreach (var user in usersPage)
        {
            var isFriendWithCurrentUser = user.IsFriendWith(currentUserId);
            var pendingFriendshipRequest = friendshipRequests.FirstOrDefault(x => x.FriendId == user.Id || x.UserId == user.Id);
            var mutualFriends = await new FriendshipService(_userRepository).GetMutualFriends(currentUser, user, currentUserFriends, cancellationToken);

            usersResult.Add(new ExtendedUserType(user, isFriendWithCurrentUser, pendingFriendshipRequest, mutualFriends));
        }

        await _userProfileImageService.PopulateImageUrl(usersResult.ToArray());


        return new PagedList<ExtendedUserType>(usersResult, request.Page, request.PageSize, totalCount, firstPageSize);
    }
}