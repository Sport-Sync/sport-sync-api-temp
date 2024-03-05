using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Common;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetFriends;

public class GetFriendsRequestHandler : IRequestHandler<GetFriendsInput, PagedList<UserType>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;

    public GetFriendsRequestHandler(IUserIdentifierProvider userIdentifierProvider, IUserRepository userRepository)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
    }

    public async Task<PagedList<UserType>> Handle(GetFriendsInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var friends = _userRepository.GetQueryable(u =>
            (u.FriendInvitees.Any(x => x.UserId == userId) || u.FriendInviters.Any(x => x.FriendId == userId)) &&
            (request.Search == null || u.FirstName.StartsWith(request.Search) || u.LastName.StartsWith(request.Search)));

        var totalCount = await friends.CountAsync(cancellationToken);

        var friendsPage = await friends
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken);

        return new PagedList<UserType>(friendsPage, request.Page, request.PageSize, totalCount);
    }
}