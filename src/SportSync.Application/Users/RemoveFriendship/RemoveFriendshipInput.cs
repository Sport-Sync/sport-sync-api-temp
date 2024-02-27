using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Users.RemoveFriendship;

public class RemoveFriendshipInput : IRequest<Result>
{
    public Guid FriendId { get; set; }
}