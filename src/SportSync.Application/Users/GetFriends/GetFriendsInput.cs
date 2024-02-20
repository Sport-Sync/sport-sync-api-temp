using SportSync.Application.Core.Common;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetFriends;

public class GetFriendsInput : IRequest<PagedList<UserType>>
{
    public string? PhoneNumber { get; set; }
    public string? Name { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}