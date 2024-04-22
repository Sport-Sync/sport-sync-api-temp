using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using static SportSync.Domain.Core.Utility.TimeZoneUtility;

namespace SportSync.Domain.Entities;

public class Match : AggregateRoot
{
    private readonly HashSet<Player> _players = new();

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
        StartTime = GetLocalDateTimeOffset(date.Date, schedule.StartTime);
        EndTime = GetLocalDateTimeOffset(date.Date, schedule.EndTime);
        Date = date;
        Status = MatchStatusEnum.Pending;
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
        StartTime = GetLocalDateTimeOffset(date.Date, match.StartTime);
        EndTime = GetLocalDateTimeOffset(date.Date, match.EndTime);
        Date = date;
        Status = match.Status;
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
    public SportTypeEnum SportType { get; set; }
    public MatchStatusEnum Status { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public string Notes { get; set; }
    public EventSchedule Schedule { get; set; }
    public MatchAnnouncement Announcement { get; set; }
    public IReadOnlyCollection<Player> Players => _players.ToList();

    public bool Announced => Announcement != null;
    public bool PubliclyAnnounced => Announced && Announcement.AnnouncementType == MatchAnnouncementTypeEnum.Public;
    public bool PrivatelyAnnounced => Announced && Announcement.AnnouncementType == MatchAnnouncementTypeEnum.FriendList;

    public static Match Create(Event @event, DateTime date, EventSchedule schedule)
    {
        return new Match(@event, date, schedule);
    }

    public static Match CreateCopy(Match match, DateTime date)
    {
        Ensure.NotNull(match.Schedule, "The schedule can not be empty for new match.",
            $"{nameof(match)}{nameof(match.Schedule)}");

        match.ThrowIfIsNotPending();

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
        ThrowIfIsNotPending();

        var player = _players.FirstOrDefault(p => p.UserId == userId);
        if (player == null)
        {
            throw new DomainException(DomainErrors.Match.PlayerNotFound);
        }

        player.Attending = attending;
    }

    public MatchAnnouncement Announce(User user, bool announcingPublicly, int numberOfPlayers, string description = null)
    {
        ThrowIfIsNotPending();

        if (!IsPlayer(user.Id))
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.UserIsNotPlayer);
        }

        if (PubliclyAnnounced)
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.AlreadyPubliclyAnnounced);
        }

        if (Announced && !announcingPublicly)
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.AlreadyAnnounced);
        }

        var type = announcingPublicly ? MatchAnnouncementTypeEnum.Public : MatchAnnouncementTypeEnum.FriendList;

        if (announcingPublicly && Announced)
        {
            Announcement.Update(user.Id, type, numberOfPlayers, description);
        }
        else
        {
            Announcement = new MatchAnnouncement(this, user.Id, type, numberOfPlayers, description);
            var player = Players.First(x => x.UserId == user.Id);
            player.SetAsMatchAnnouncer();
        }

        if (!announcingPublicly)
        {
            RaiseDomainEvent(new MatchAnnouncedToFriendsDomainEvent(this, user));
        }

        return Announcement;
    }

    public async Task<Result> CancelAnnouncement(Guid userId, IEventRepository eventRepository, CancellationToken cancellationToken)
    {
        if (!Announced)
        {
            return Result.Failure(DomainErrors.MatchAnnouncement.NotAnnounced);
        }

        if (!IsPlayer(userId))
        {
            return Result.Failure(DomainErrors.MatchAnnouncement.UserIsNotPlayer);
        }

        var userIsAdmin = await eventRepository.IsAdminOnEvent(EventId, userId, cancellationToken);

        if (PrivatelyAnnounced)
        {
            if (Announcement.UserId != userId && !userIsAdmin)
            {
                return Result.Failure(DomainErrors.User.Forbidden);
            }
        }

        if (PubliclyAnnounced && !userIsAdmin)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        Announcement.Delete();

        return Result.Success();
    }

    public void RemoveAnnouncement()
    {
        if (!Announced)
        {
            return;
        }

        Announcement.Delete();
    }

    public void ThrowIfIsNotPending()
    {
        var isPendingResult = ValidateItIsPendingStatus();
        if (isPendingResult.IsFailure)
        {
            throw new DomainException(isPendingResult.Error);
        }
    }

    public Result<MatchApplication> ApplyForPlaying(User user)
    {
        var isPendingResult = ValidateItIsPendingStatus();
        if (isPendingResult.IsFailure)
        {
            return Result.Failure<MatchApplication>(isPendingResult.Error);
        }

        var canApply = CanUserApplyForMatch(user);

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

    private Result CanUserApplyForMatch(User user)
    {
        if (IsPlayer(user.Id))
        {
            return Result.Failure<MatchApplication>(DomainErrors.Match.AlreadyPlayer);
        }

        if (!Announced)
        {
            return Result.Failure<MatchApplication>(DomainErrors.MatchApplication.NotAnnounced);
        }

        if (PubliclyAnnounced)
        {
            return Result.Success();
        }

        var announcerIds = Players
            .Where(x => x.HasAnnouncedMatch)
            .Select(p => p.UserId);

        if (!user.Friends.Any(friendId => announcerIds.Contains(friendId)))
        {
            return Result.Failure(DomainErrors.MatchApplication.NotOnFriendList);
        }

        return Result.Success();
    }

    public void SetStatus(MatchStatusEnum status)
    {
        Status = status;

        if (Status == MatchStatusEnum.Finished)
        {
            // TODO: send push notifications to creator to enter the result
            //RaiseDomainEvent(new MatchFinishedDomainEvent(this));
        }
    }

    public Result ValidateItIsPendingStatus()
    {
        if (Status == MatchStatusEnum.Finished)
        {
            return Result.Failure(DomainErrors.Match.AlreadyFinished);
        }

        if (Status == MatchStatusEnum.InProgress)
        {
            return Result.Failure(DomainErrors.Match.InProgress);
        }

        if (Status == MatchStatusEnum.Canceled)
        {
            return Result.Failure(DomainErrors.Match.Canceled);
        }

        return Result.Success();
    }
}