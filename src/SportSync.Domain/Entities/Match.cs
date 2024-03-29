using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class Match : AggregateRoot
{
    private readonly HashSet<Player> _players = new();
    private readonly HashSet<MatchAnnouncement> _announcements = new();

    private Match(Event @event, DateTime date, EventSchedule schedule)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(@event, "The event is required.", nameof(@event));
        Ensure.NotEmpty(@event.Id, "The event identifier is required.", $"{nameof(@event)}{nameof(@event.Id)}");

        EventId = @event.Id;
        ScheduleId = schedule.Id;
        EventName = @event.Name;
        Address = @event.Address;
        SportType = @event.SportType;
        Price = @event.Price;
        NumberOfPlayersExpected = @event.NumberOfPlayers;
        Notes = @event.Notes;
        StartTime = new DateTimeOffset(date.Date.Add(schedule.StartTime.TimeOfDay));
        EndTime = new DateTimeOffset(date.Date.Add(schedule.EndTime.TimeOfDay));
        Date = date;
        Status = MatchStatus.Pending;
    }

    private Match(Match match, DateTime date)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(match, "The match is required.", nameof(match));

        EventId = match.EventId;
        ScheduleId = match.ScheduleId;
        EventName = match.EventName;
        Address = match.Address;
        SportType = match.SportType;
        Price = match.Price;
        NumberOfPlayersExpected = match.NumberOfPlayersExpected;
        Notes = match.Notes;
        StartTime = new DateTimeOffset(date.Date.Add(match.StartTime.TimeOfDay));
        EndTime = new DateTimeOffset(date.Date.Add(match.EndTime.TimeOfDay));
        Date = date;
    }

    private Match()
    {

    }

    public Guid EventId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTime Date { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string EventName { get; set; }
    public SportType SportType { get; set; }
    public MatchStatus Status { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public string Notes { get; set; }

    public EventSchedule Schedule { get; set; }
    public IReadOnlyCollection<Player> Players => _players.ToList();
    public IReadOnlyCollection<MatchAnnouncement> Announcements => _announcements.ToList();

    public bool Announced => _announcements.Any();
    public bool PubliclyAnnounced => _announcements.Any(x => x.AnnouncementType == MatchAnnouncementType.Public);

    public static Match Create(Event @event, DateTime date, EventSchedule schedule)
    {
        return new Match(@event, date, schedule);
    }

    public static Match CreateCopy(Match match, DateTime date)
    {
        Ensure.NotNull(match.Schedule, "The schedule can not be empty for new match.",
            $"{nameof(match)}{nameof(match.Schedule)}");

        match.EnsureItIsNotDone();

        var newMatch = new Match(match, date);

        var playerIds = match.Players.Select(p => p.UserId).ToList();

        newMatch.AddPlayers(playerIds);

        return newMatch;
    }

    public void AddPlayer(Guid userId) => AddPlayers(new List<Guid>() { userId });

    public void AddPlayers(List<Guid> userIds)
    {
        foreach (Guid userId in userIds)
        {
            _players.Add(Player.Create(userId, Id));
        }
    }

    public void SetPlayerAttendance(Guid userId, bool attending)
    {
        EnsureItIsNotDone();

        var player = _players.FirstOrDefault(p => p.UserId == userId);
        if (player == null)
        {
            throw new DomainException(DomainErrors.Match.PlayerNotFound);
        }

        player.Attending = attending;
    }

    public MatchAnnouncement Announce(User user, bool publicly)
    {
        EnsureItIsNotDone();

        if (PubliclyAnnounced)
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.AlreadyPubliclyAnnounced);
        }

        if (publicly)
        {
            _announcements.RemoveWhere(x => x.AnnouncementType == MatchAnnouncementType.FriendList);
        }

        if (!publicly && _announcements.Any(x => x.UserId == user.Id))
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.AlreadyAnnouncedBySameUser);
        }

        var type = publicly ? MatchAnnouncementType.Public : MatchAnnouncementType.FriendList;
        var announcement = new MatchAnnouncement(this, user.Id, type);

        _announcements.Add(announcement);

        if (!publicly)
        {
            RaiseDomainEvent(new MatchAnnouncedToFriendsDomainEvent(this, user));
        }

        return announcement;
    }

    public void EnsureItIsNotDone()
    {
        bool finishedStatus = Status switch
        {
            MatchStatus.Finished => true,
            MatchStatus.Canceled => true,
            _ => false
        };

        if (finishedStatus)
        {
            throw new DomainException(DomainErrors.Match.AlreadyFinished);
        }

        if (HasPassed())
        {
            throw new DomainException(DomainErrors.Match.AlreadyFinished);
        }
    }

    public bool HasPassed(DateTime? dateTime = null)
    {
        var date = dateTime ?? DateTime.UtcNow;

        if (Date < date.Date)
        {
            return true;
        }

        if (Date == date.Date && StartTime.TimeOfDay <= date.TimeOfDay)
        {
            return true;
        }

        return false;
    }

    public Result<MatchApplication> ApplyForPlaying(User user)
    {
        var canApply = IsValidApplicant(user);

        if (canApply.IsFailure)
        {
            return Result.Failure<MatchApplication>(canApply.Error);
        }

        var matchApplication = MatchApplication.Create(user, this);

        RaiseDomainEvent(new MatchApplicationSentDomainEvent(matchApplication, this));

        return matchApplication;
    }

    public bool IsPlayer(Guid userId)
    {
        return _players.Any(x => x.UserId == userId);
    }

    private Result IsValidApplicant(User user)
    {
        if (IsPlayer(user.Id))
        {
            return Result.Failure<MatchApplication>(DomainErrors.MatchApplication.AlreadyPlayer);
        }

        if (!Announced)
        {
            return Result.Failure(DomainErrors.MatchApplication.NotAnnounced);
        }

        if (PubliclyAnnounced)
        {
            return Result.Success();
        }

        var privateAnnouncements =
            Announcements
            .Where(a => a.AnnouncementType == MatchAnnouncementType.FriendList)
            .ToList();

        var announcerIds = privateAnnouncements.Select(x => x.UserId);

        if (!user.Friends.Any(friendId => announcerIds.Contains(friendId)))
        {
            return Result.Failure(DomainErrors.MatchApplication.NotOnFriendList);
        }

        return Result.Success();
    }
}