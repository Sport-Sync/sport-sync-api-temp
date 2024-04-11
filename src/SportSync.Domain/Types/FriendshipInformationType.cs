namespace SportSync.Domain.Types;

public class FriendshipInformationType
{
    public List<UserType> MutualFriends { get; set; } = new();
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }
    public bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;
    public bool IsFriendWithCurrentUser { get; set; }

    public FriendshipInformationType(List<UserType> mutualFriends, PendingFriendshipRequestType pendingFriendshipRequest, bool friendWithCurrentUser)
    {
        MutualFriends = mutualFriends;
        PendingFriendshipRequest = pendingFriendshipRequest;
        IsFriendWithCurrentUser = friendWithCurrentUser;
    }
}