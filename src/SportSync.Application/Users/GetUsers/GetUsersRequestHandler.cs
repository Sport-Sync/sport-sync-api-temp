using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Core.Common;
using SportSync.Application.Core.Constants;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUsers;

public class GetUsersRequestHandler : IRequestHandler<GetUsersInput, PagedList<UserType>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetUsersRequestHandler(IUserIdentifierProvider userIdentifierProvider, IUserRepository userRepository, IBlobStorageService blobStorageService)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<PagedList<UserType>> Handle(GetUsersInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var users = _userRepository.GetQueryable<UserType>(
            UserType.PropertySelector,
            u => (request.Search == null || u.FirstName.StartsWith(request.Search) || u.LastName.StartsWith(request.Search)))
            .OrderBy(u => u.FirstName);

        var totalCount = await users.CountAsync(cancellationToken);

        var firstPageSize = request.FirstPageSize ?? PaginationConstants.FirstPageSize;

        var skip = request.Page == 1 ? 0 :
            firstPageSize + ((request.Page - 2) * request.PageSize);

        var friendsPage = await users
            .Skip(skip)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken);

        foreach (var friend in friendsPage.Where(f => f.HasProfileImage))
        {
            friend.ImageUrl = await _blobStorageService.GetProfileImageUrl(friend.Id);
        }

        return new PagedList<UserType>(friendsPage, request.Page, request.PageSize, totalCount, firstPageSize);
    }
}