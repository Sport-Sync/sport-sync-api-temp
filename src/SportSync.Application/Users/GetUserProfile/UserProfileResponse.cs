using SportSync.Domain.Entities;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUserProfile;

public class UserProfileResponse
{
    public UserType User { get; set; }
    public List<UserType> MutualFriends { get; set; } = new ();
    public PendingFriendshipRequestType? PendingFriendshipRequest { get; set; }
    public bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;
    public bool IsFriendWithCurrentUser { get; set; }

    public UserProfileResponse(User user, string? imageUrl = null)
    {
        User = new UserType(user, imageUrl);
    }

    public UserProfileResponse()
    {
        
    }
}