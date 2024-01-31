using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class Termin : AggregateRoot
{
    private readonly HashSet<Player> _players = new();

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

    public void Announce(bool publicly)
    {
        EnsureItIsNotDone();

        Status = publicly ? TerminStatus.AnnouncedPublicly : TerminStatus.AnnouncedInternally;
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
}