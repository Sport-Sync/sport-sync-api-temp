using SportSync.Domain.Entities;
using SportSync.Domain.Types.Abstraction;

namespace SportSync.Domain.Types;

public class UserProfileType : UserType, IPendingFriendshipRequestsInfo
{
    public List<UserType> MutualFriends { get; set; }
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }
    public bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;
    public bool IsFriendWithCurrentUser { get; set; }

    public UserProfileType(User user, PendingFriendshipRequestType pendingFriendshipRequest = null, List<UserType> mutualFriends = null, string imageUrl = null)
        : base(user, imageUrl)
    {

        PendingFriendshipRequest = pendingFriendshipRequest;
        MutualFriends = mutualFriends;
    }

    public UserProfileType(User user, string imageUrl = null)
        : base(user)
    {
        ImageUrl = imageUrl;
    }

    public UserProfileType()
    {
        
    }
}