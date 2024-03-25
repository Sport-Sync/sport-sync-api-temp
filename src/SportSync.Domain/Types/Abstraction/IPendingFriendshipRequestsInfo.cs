namespace SportSync.Domain.Types.Abstraction;

public interface IPendingFriendshipRequestsInfo
{
    PendingFriendshipRequestType PendingFriendshipRequest { get; set; }
    bool HasPendingFriendshipRequest { get; }
}