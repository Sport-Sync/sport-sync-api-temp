using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class ExtendedUserType : UserType
{
    public List<UserType> MutualFriends { get; set; } = new();
    public Guid? PendingFriendshipRequestId { get; set; }
    public bool IsFriendshipRequestSentByCurrentUser { get; set; }
    public bool IsFriendWithCurrentUser { get; set; }

    public ExtendedUserType(User user, bool friendWithCurrentUser, FriendshipRequest friendshipRequestWithCurrentUser, List<UserType> mutualFriends)
        : base(user)
    {
        MutualFriends = mutualFriends;
        PendingFriendshipRequestId = friendshipRequestWithCurrentUser?.Id;
        IsFriendshipRequestSentByCurrentUser = !friendshipRequestWithCurrentUser?.IsSender(user.Id) ?? false;
        IsFriendWithCurrentUser = friendWithCurrentUser;
    }

    public ExtendedUserType(User user, bool friendWithCurrentUser, FriendshipRequest friendshipRequestWithCurrentUser)
        : base(user)
    {
        PendingFriendshipRequestId = friendshipRequestWithCurrentUser?.Id;
        IsFriendshipRequestSentByCurrentUser = !friendshipRequestWithCurrentUser?.IsSender(user.Id) ?? false;
        IsFriendWithCurrentUser = friendWithCurrentUser;
    }

    public ExtendedUserType(User user)
         : base(user)
    {

    }

    public ExtendedUserType()
    {

    }
}