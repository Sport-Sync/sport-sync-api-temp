using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class Termin : AggregateRoot
{
    private readonly HashSet<Player> _players = new();
    private readonly HashSet<TerminAnnouncement> _announcements = new();

    private Termin(Event @event, DateTime date, EventSchedule schedule)
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
        StartTime = schedule.StartTime;
        EndTime = schedule.EndTime;
        Date = date;
        Status = TerminStatus.Pending;
    }

    private Termin(Termin termin, DateTime date)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(termin, "The termin is required.", nameof(termin));

        EventId = termin.EventId;
        ScheduleId = termin.ScheduleId;
        EventName = termin.EventName;
        Address = termin.Address;
        SportType = termin.SportType;
        Price = termin.Price;
        NumberOfPlayersExpected = termin.NumberOfPlayersExpected;
        Notes = termin.Notes;
        StartTime = termin.StartTime;
        EndTime = termin.EndTime;
        Date = date;
    }

    private Termin()
    {

    }

    public Guid EventId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string EventName { get; set; }
    public SportType SportType { get; set; }
    public TerminStatus Status { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public string Notes { get; set; }

    public EventSchedule Schedule { get; set; }
    public IReadOnlyCollection<Player> Players => _players.ToList();
    public IReadOnlyCollection<TerminAnnouncement> Announcements => _announcements.ToList();

    public bool Announced => _announcements.Any();
    public bool PubliclyAnnounced => _announcements.Any(x => x.AnnouncementType == TerminAnnouncementType.Public);

    public static Termin Create(Event @event, DateTime date, EventSchedule schedule)
    {
        return new Termin(@event, date, schedule);
    }

    public static Termin CreateCopy(Termin termin, DateTime date)
    {
        Ensure.NotNull(termin.Schedule, "The schedule can not be empty for new termin.",
            $"{nameof(termin)}{nameof(termin.Schedule)}");

        termin.EnsureItIsNotDone();

        var newTermin = new Termin(termin, date);

        var playerIds = termin.Players.Select(p => p.UserId).ToList();

        newTermin.AddPlayers(playerIds);

        return newTermin;
    }

    public void AddPlayer(Guid userId) => AddPlayers(new List<Guid>() { userId });

    public void AddPlayers(List<Guid> userIds)
    {
        foreach (Guid userId in userIds)
        {
            _players.Add(Player.Create(userId, Id));
        }
    }

    public void SetPlayerAttendence(Guid userId, bool attending)
    {
        EnsureItIsNotDone();

        var player = _players.FirstOrDefault(p => p.UserId == userId);
        if (player == null)
        {
            throw new DomainException(DomainErrors.Termin.PlayerNotFound);
        }

        player.Attending = attending;
    }

    public TerminAnnouncement Announce(Guid userId, bool publicly)
    {
        EnsureItIsNotDone();

        if (PubliclyAnnounced)
        {
            throw new DomainException(DomainErrors.TerminAnnouncement.AlreadyPubliclyAnnounced);
        }

        if (publicly)
        {
            _announcements.RemoveWhere(x => x.AnnouncementType == TerminAnnouncementType.FriendList);
        }

        if (!publicly && _announcements.Any(x => x.UserId == userId))
        {
            throw new DomainException(DomainErrors.TerminAnnouncement.AlreadyAnnouncedBySameUser);
        }

        var type = publicly ? TerminAnnouncementType.Public : TerminAnnouncementType.FriendList;
        var announcement = new TerminAnnouncement(this, userId, type);

        _announcements.Add(announcement);

        return announcement;
    }

    public void EnsureItIsNotDone()
    {
        bool finishedStatus = Status switch
        {
            TerminStatus.Finished => true,
            TerminStatus.Canceled => true,
            _ => false
        };

        if (finishedStatus)
        {
            throw new DomainException(DomainErrors.Termin.AlreadyFinished);
        }

        if (Date < DateTime.Today)
        {
            throw new DomainException(DomainErrors.Termin.AlreadyFinished);
        }

        if (Date == DateTime.Today && StartTime.TimeOfDay <= DateTime.UtcNow.TimeOfDay)
        {
            throw new DomainException(DomainErrors.Termin.AlreadyFinished);
        }
    }

    public Result<TerminApplication> ApplyForPlaying(User user)
    {
        var canApply = IsValidApplicant(user);

        if (canApply.IsFailure)
        {
            return Result.Failure<TerminApplication>(canApply.Error);
        }

        var terminApplication = new TerminApplication(user, this);

        RaiseDomainEvent(new TerminApplicationSentDomainEvent(terminApplication, this));

        return terminApplication;
    }

    public bool IsPlayer(Guid userId)
    {
        return _players.Any(x => x.UserId == userId);
    }

    private Result IsValidApplicant(User user)
    {
        if (IsPlayer(user.Id))
        {
            return Result.Failure<TerminApplication>(DomainErrors.TerminApplication.AlreadyPlayer);
        }

        if (!Announced)
        {
            return Result.Failure(DomainErrors.TerminApplication.NotAnnounced);
        }

        if (PubliclyAnnounced)
        {
            return Result.Success();
        }

        var privateAnnouncements =
            Announcements
            .Where(a => a.AnnouncementType == TerminAnnouncementType.FriendList)
            .ToList();

        var announcerIds = privateAnnouncements.Select(x => x.UserId);

        if (!user.Friends.Any(friendId => announcerIds.Contains(friendId)))
        {
            return Result.Failure(DomainErrors.TerminApplication.NotOnFriendList);
        }

        return Result.Success();
    }
}