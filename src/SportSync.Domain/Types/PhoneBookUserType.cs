using HotChocolate;
using SportSync.Domain.Types.Abstraction;

namespace SportSync.Domain.Types;

public class PhoneBookUserType : UserType, IPendingFriendshipRequestsInfo
{
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }
    public bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;

    [GraphQLIgnore]
    public new string ImageUrl { get; set; }

    public PhoneBookUserType(UserType user, PendingFriendshipRequestType pendingFriendshipRequest = null)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
        PendingFriendshipRequest = pendingFriendshipRequest;
    }

    public PhoneBookUserType()
    {
        
    }
}