using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;

namespace SportSync.Domain.Entities;

public class EventInvitation : Entity
{
    private EventInvitation(User byUser, User toUser, Event @event)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(byUser, "The user is required.", nameof(byUser));
        Ensure.NotEmpty(byUser.Id, "The user identifier is required.", $"{nameof(byUser)}{nameof(byUser.Id)}");
        Ensure.NotNull(toUser, "The user is required.", nameof(toUser));
        Ensure.NotEmpty(toUser.Id, "The user identifier is required.", $"{nameof(toUser)}{nameof(toUser.Id)}");
        Ensure.NotNull(@event, "The event is required.", nameof(@event));
        Ensure.NotEmpty(@event.Id, "The event identifier is required.", $"{nameof(@event)}{nameof(@event.Id)}");

        SentByUserId = byUser.Id;
        SentToUserId = toUser.Id;
        EventId = @event.Id;
    }

    private EventInvitation()
    {
    }

    public Guid SentByUserId { get; set; }

    public Guid SentToUserId { get; set; }

    public User SentByUser { get; set; }

    public User SentToUser { get; set; }

    public Guid EventId { get; set; }

    public bool Accepted { get; private set; }

    public bool Rejected { get; private set; }

    public DateTime? CompletedOnUtc { get; private set; }

    public static EventInvitation Create(User byUser, User toUser, Event @event)
    {
        return new EventInvitation(byUser, toUser, @event);
    }

    public Result Accept(User user, DateTime utcNow)
    {
        if (SentToUserId != user.Id)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        if (Accepted)
        {
            return Result.Failure(DomainErrors.EventInvitation.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.EventInvitation.AlreadyRejected);
        }

        Accepted = true;
        CompletedOnUtc = utcNow;

        //RaiseDomainEvent(new TerminApplicationAcceptedDomainEvent(this));

        return Result.Success();
    }

    public Result Reject(User user, DateTime utcNow)
    {
        if (SentToUserId != user.Id)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        if (Accepted)
        {
            return Result.Failure(DomainErrors.EventInvitation.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.EventInvitation.AlreadyRejected);
        }

        Rejected = true;
        CompletedOnUtc = utcNow;

        return Result.Success();
    }
}