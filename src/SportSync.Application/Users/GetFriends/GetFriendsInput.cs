using SportSync.Application.Core.Common;
using SportSync.Application.Core.Constants;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetFriends;

public class GetFriendsInput : IRequest<PagedList<UserType>>
{
    public string? Search { get; set; }
    public int Page { get; set; }
    public int? FirstPageSize { get; set; } = PaginationConstants.FirstPageSize;
    public int PageSize { get; set; }
}