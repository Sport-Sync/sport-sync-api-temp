using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class Termin : AggregateRoot
{
    private readonly HashSet<Player> _players = new();

    private Termin(Event @event, DateOnly date, EventSchedule schedule)
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
        StartTimeUtc = schedule.StartTimeUtc;
        EndTimeUtc = schedule.EndTimeUtc;
        Date = date;
    }

    private Termin(Termin termin, DateOnly date)
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
        StartTimeUtc = termin.StartTimeUtc;
        EndTimeUtc = termin.EndTimeUtc;
        Date = date;
    }

    private Termin()
    {

    }

    public Guid EventId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTimeUtc { get; set; }
    public TimeOnly EndTimeUtc { get; set; }
    public string EventName { get; set; }
    public SportType SportType { get; set; }
    public TerminStatus Status { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public string Notes { get; set; }

    public EventSchedule Schedule { get; set; }
    public IReadOnlyCollection<Player> Players => _players.ToList();

    public static Termin Create(Event @event, DateOnly date, EventSchedule schedule)
    {
        return new Termin(@event, date, schedule);
    }

    public static Termin CreateCopy(Termin termin, DateOnly date)
    {
        Ensure.NotNull(termin.Schedule, "The schedule can not be empty for new termin.", $"{nameof(termin)}{nameof(termin.Schedule)}");

        if (termin.Status != TerminStatus.Open)
        {
            throw new DomainException(DomainErrors.Termin.NotOpen);
        }

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
        if (Date < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new DomainException(DomainErrors.Termin.AlreadyFinished);
        }

        if (Date == DateOnly.FromDateTime(DateTime.Today) && 
            StartTimeUtc <= TimeOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new DomainException(DomainErrors.Termin.AlreadyFinished);
        }

        var player = _players.FirstOrDefault(p => p.UserId == userId);
        if (player == null)
        {
            throw new DomainException(DomainErrors.Termin.PlayerNotFound);
        }

        player.Attending = attending;
    }
}