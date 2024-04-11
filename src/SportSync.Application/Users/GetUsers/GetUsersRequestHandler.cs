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

public class GetUsersRequestHandler : IRequestHandler<GetUsersInput, PagedList<UserType>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileImageService _userProfileImageService;

    public GetUsersRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IUserProfileImageService userProfileImageService)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _userProfileImageService = userProfileImageService;
    }

    public async Task<PagedList<UserType>> Handle(GetUsersInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var allUsersQuery = _userRepository.GetQueryable<UserType>(
                UserType.PropertySelector,
                u => u.Id != userId && (request.Search == null || u.FirstName.StartsWith(request.Search) || u.LastName.StartsWith(request.Search)))
            .OrderBy(u => u.FirstName);

        var friendListQuery = _userRepository.GetQueryable<UserType>(
                UserType.PropertySelector,
                u => u.Id != userId && 
                     (u.FriendInvitees.Any(x => x.UserId == userId) || u.FriendInviters.Any(x => x.FriendId == userId)) &&
                     (request.Search == null || u.FirstName.StartsWith(request.Search) || u.LastName.StartsWith(request.Search)))
            .OrderBy(u => u.FirstName);

        var users = request.SearchType == UsersSearchType.AllUsers ? allUsersQuery : friendListQuery;

        var totalCount = await users.CountAsync(cancellationToken);

        var firstPageSize = request.FirstPageSize ?? PaginationConstants.FirstPageSize;

        var skip = request.Page == 1 ? 0 :
            firstPageSize + ((request.Page - 2) * request.PageSize);

        var usersPage = await users
            .Skip(skip)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken);

        await _userProfileImageService.PopulateImageUrl(usersPage);

        return new PagedList<UserType>(usersPage, request.Page, request.PageSize, totalCount, firstPageSize);
    }
}