using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.DomainEvents;
using static SportSync.Domain.Core.Errors.DomainErrors;

namespace SportSync.Domain.Entities;

public class MatchApplication : AggregateRoot
{
    private MatchApplication(User user, Match match)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(user, "The user is required.", nameof(user));
        Ensure.NotEmpty(user.Id, "The user identifier is required.", $"{nameof(user)}{nameof(user.Id)}");
        Ensure.NotNull(match, "The match is required.", nameof(match));
        Ensure.NotEmpty(match.Id, "The match identifier is required.", $"{nameof(match)}{nameof(match.Id)}");

        AppliedByUser = user;
        AppliedByUserId = user.Id;
        MatchId = match.Id;
    }

    private MatchApplication()
    {
    }

    public Guid AppliedByUserId { get; set; }
    
    public Guid? CompletedByUserId { get; set; }

    public User AppliedByUser { get; set; }

    public User CompletedByUser { get; set; }

    public Guid MatchId { get; set; }

    public bool Accepted { get; private set; }

    public bool Rejected { get; private set; }

    public bool Canceled { get; private set; }

    public DateTime? CompletedOnUtc { get; private set; }

    public bool Completed => CompletedOnUtc != null;

    public static MatchApplication Create(User user, Match match)
    {
        return new MatchApplication(user, match);
    }

    public Result Accept(User user, Match match, DateTime utcNow)
    {
        if (Accepted)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyRejected);
        }

        if (Canceled)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyCanceled);
        }

        var matchAnnouncement = match.Announcement;

        if (matchAnnouncement.AcceptedPlayersCount >= matchAnnouncement.PlayerLimit)
        {
            return Result.Failure(DomainErrors.MatchApplication.PlayersLimitReached);
        }

        Accepted = true;

        CompletedOnUtc = utcNow;
        CompletedByUserId = user.Id;
        CompletedByUser = user;

        RaiseDomainEvent(new MatchApplicationAcceptedDomainEvent(this));

        return Result.Success();
    }

    public Result Reject(User user, DateTime utcNow)
    {
        if (Accepted)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyRejected);
        }

        if (Canceled)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyCanceled);
        }

        Rejected = true;

        CompletedOnUtc = utcNow;
        CompletedByUserId = user.Id;

        return Result.Success();
    }

    public Result Cancel(User user, DateTime utcNow)
    {
        if (AppliedByUserId != user.Id)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        if (Accepted)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyAccepted);
        }

        if (Rejected)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyRejected);
        }

        if (Canceled)
        {
            return Result.Failure(DomainErrors.MatchApplication.AlreadyCanceled);
        }

        Canceled = true;

        CompletedOnUtc = utcNow;
        CompletedByUserId = user.Id;

        RaiseDomainEvent(new MatchApplicationCanceledDomainEvent(this));

        return Result.Success();
    }
}