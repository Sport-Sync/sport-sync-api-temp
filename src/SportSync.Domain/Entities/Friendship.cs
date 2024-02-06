using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;

namespace SportSync.Domain.Entities;

public class Friendship : Entity
{
    public Friendship(User user, User friend)
    {
        Ensure.NotNull(user, "The user is required.", nameof(user));
        Ensure.NotEmpty(user.Id, "The user identifier is required.", $"{nameof(user)}{nameof(user.Id)}");
        Ensure.NotNull(friend, "The friend is required.", nameof(friend));
        Ensure.NotEmpty(friend.Id, "The friend identifier is required.", $"{nameof(friend)}{nameof(friend.Id)}");

        UserId = user.Id;
        FriendId = friend.Id;
    }
    
    private Friendship()
    {
    }
    
    public Guid UserId { get; }
    
    public Guid FriendId { get; }
}