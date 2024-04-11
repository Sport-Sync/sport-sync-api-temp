using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Common;
using SportSync.Application.Core.Constants;
using SportSync.Application.Core.Services;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUsers;

public class GetUsersRequestHandler : IRequestHandler<GetUsersInput, PagedList<UserProfileType>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserProfileImageService _userProfileImageService;
    private readonly IFriendshipInformationService _friendshipInformationService;

    public GetUsersRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IUserProfileImageService userProfileImageService,
        IFriendshipRequestRepository friendshipRequestRepository,
        IFriendshipInformationService friendshipInformationService)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _userProfileImageService = userProfileImageService;
        _friendshipRequestRepository = friendshipRequestRepository;
        _friendshipInformationService = friendshipInformationService;
    }

    public async Task<PagedList<UserProfileType>> Handle(GetUsersInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var currentUser = maybeUser.Value;

        var usersQuery = _userRepository.GetQueryableWhere(u =>
                u.Id != userId &&
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

        var usersProfileTypes = usersPage.Select(x => new UserProfileType(x)).ToArray();

        await _userProfileImageService.PopulateImageUrl(usersProfileTypes);

        var currentUserFriends = await _userRepository.GetByIdsAsync(currentUser.Friends.ToList(), cancellationToken);

        foreach (var user in usersPage)
        {
            var friendshipInformation = await _friendshipInformationService.GetFriendshipInformationForCurrentUser(currentUser, user, currentUserFriends, _friendshipRequestRepository, _userProfileImageService, cancellationToken);

            var userProfileType = usersProfileTypes.First(x => x.Id == user.Id);
            userProfileType.FriendshipInformation = friendshipInformation;
        }

        return new PagedList<UserProfileType>(usersProfileTypes, request.Page, request.PageSize, totalCount, firstPageSize);
    }
}