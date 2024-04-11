using SportSync.Application.Core.Common;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUsers;

public class GetUsersInput : PaginationInput, IRequest<PagedList<UserProfileType>>
{
    public string? Search { get; set; }
}