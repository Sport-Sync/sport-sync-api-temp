using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.DomainEvents;

namespace SportSync.Domain.Entities;

public class FriendshipRequest : AggregateRoot
{
    public FriendshipRequest(User user, User friend)
       : base(Guid.NewGuid())
    {
        Ensure.NotNull(user, "The user is required.", nameof(user));
        Ensure.NotEmpty(user.Id, "The user identifier is required.", $"{nameof(user)}{nameof(user.Id)}");
        Ensure.NotNull(friend, "The friend is required.", nameof(friend));
        Ensure.NotEmpty(friend.Id, "The friend identifier is required.", $"{nameof(friend)}{nameof(friend.Id)}");

        UserId = user.Id;
        FriendId = friend.Id;
    }

    private FriendshipRequest()
    {
    }

    public Guid UserId { get; }

    public Guid FriendId { get; }

    public bool Accepted { get; private set; }

    public bool Rejected { get; private set; }

    public User User { get; set; }

    public DateTime? CompletedOnUtc { get; private set; }
    
    public Result Accept(DateTime utcNow)
    {
        if (Accepted)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.AlreadyRejected);
        }

        Accepted = true;

        CompletedOnUtc = utcNow;

        RaiseDomainEvent(new FriendshipRequestAcceptedDomainEvent(this));

        return Result.Success();
    }
    
    public Result Reject(DateTime utcNow)
    {
        if (Accepted)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.AlreadyRejected);
        }

        Rejected = true;

        CompletedOnUtc = utcNow;

        RaiseDomainEvent(new FriendshipRequestRejectedDomainEvent(this));

        return Result.Success();
    }

    public bool IsSender(Guid userId)
    {
        return UserId == userId;
    }
}