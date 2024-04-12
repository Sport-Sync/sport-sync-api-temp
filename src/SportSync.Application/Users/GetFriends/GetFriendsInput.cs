using SportSync.Application.Core.Common;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetFriends;

public class GetFriendsInput : PaginationInput, IRequest<PagedList<ExtendedUserType>>
{
    public string? Search { get; set; }
}