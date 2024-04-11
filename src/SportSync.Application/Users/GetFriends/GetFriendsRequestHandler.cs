using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Common;
using SportSync.Application.Core.Constants;
using SportSync.Application.Core.Services;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetFriends;

public class GetFriendsRequestHandler : IRequestHandler<GetFriendsInput, PagedList<UserType>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileImageService _userProfileImageService;

    public GetFriendsRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IUserProfileImageService userProfileImageService)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _userProfileImageService = userProfileImageService;
    }

    public async Task<PagedList<UserType>> Handle(GetFriendsInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var friends = _userRepository.GetQueryable<UserType>(
            UserType.PropertySelector,
            u => (u.FriendInvitees.Any(x => x.UserId == userId) || u.FriendInviters.Any(x => x.FriendId == userId)) &&
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

        await _userProfileImageService.PopulateImageUrl(friendsPage);

        return new PagedList<UserType>(friendsPage, request.Page, request.PageSize, totalCount, firstPageSize);
    }
}