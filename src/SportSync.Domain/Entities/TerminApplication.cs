using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;

namespace SportSync.Domain.Entities;

public class TerminApplication : AggregateRoot
{
    public TerminApplication(User user, Termin termin)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(user, "The user is required.", nameof(user));
        Ensure.NotEmpty(user.Id, "The user identifier is required.", $"{nameof(user)}{nameof(user.Id)}");
        Ensure.NotNull(termin, "The termin is required.", nameof(termin));
        Ensure.NotEmpty(termin.Id, "The termin identifier is required.", $"{nameof(termin)}{nameof(termin.Id)}");

        UserId = user.Id;
        TerminId = termin.Id;
    }

    private TerminApplication()
    {
    }

    public Guid UserId { get; set; }

    public Guid TerminId { get; set; }

    public bool Accepted { get; private set; }

    public bool Rejected { get; private set; }

    public DateTime? CompletedOnUtc { get; private set; }

    public Result Accept(DateTime utcNow)
    {
        if (Accepted)
        {
            return Result.Failure(DomainErrors.TerminApplication.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.TerminApplication.AlreadyRejected);
        }

        Accepted = true;

        CompletedOnUtc = utcNow;

        //RaiseDomainEvent(new TerminApplicationAcceptedDomainEvent(this));

        return Result.Success();
    }

    public Result Reject(DateTime utcNow)
    {
        if (Accepted)
        {
            return Result.Failure(DomainErrors.TerminApplication.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.TerminApplication.AlreadyRejected);
        }

        Rejected = true;

        CompletedOnUtc = utcNow;

        //RaiseDomainEvent(new FriendshipRequestRejectedDomainEvent(this));

        return Result.Success();
    }
}