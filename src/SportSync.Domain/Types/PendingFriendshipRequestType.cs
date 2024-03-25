namespace SportSync.Domain.Types
{
    public class PendingFriendshipRequestType
    {
        public Guid FriendshipRequestId { get; set; }
        public bool SentByMe { get; set; }

        public static PendingFriendshipRequestType Create(Guid friendshipRequestId, bool sentByMe) =>
            new()
            {
                FriendshipRequestId = friendshipRequestId,
                SentByMe = sentByMe
            };
    }
}