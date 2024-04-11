using SportSync.Application.Core.Common;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUsers;

public class GetUsersInput : PaginationInput, IRequest<PagedList<UserType>>
{
    public string? Search { get; set; }
    public UsersSearchType SearchType { get; set; }
}

public enum UsersSearchType
{
    FriendList,
    AllUsers
}