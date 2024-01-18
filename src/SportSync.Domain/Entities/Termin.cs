using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class Termin : Entity
{
    private readonly HashSet<Player> _players = new();

    private Termin(Event @event, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        : base(Guid.NewGuid())
    {
        Ensure.NotNull(@event, "The event is required.", nameof(@event));
        Ensure.NotEmpty(@event.Id, "The event identifier is required.", $"{nameof(@event)}{nameof(@event.Id)}");

        EventId = @event.Id;
        EventName = @event.Name;
        Address = @event.Address;
        SportType = @event.SportType;
        Price = @event.Price;
        NumberOfPlayersExpected = @event.NumberOfPlayers;
        Notes = @event.Notes;
        Date = date;
        StartTimeUtc = startTime;
        EndTimeUtc = endTime;
    }

    private Termin()
    {

    }

    public Guid EventId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTimeUtc { get; set; }
    public TimeOnly EndTimeUtc { get; set; }
    public string EventName { get; set; }
    public SportType SportType { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public string Notes { get; set; }

    public IReadOnlyCollection<Player> Players => _players.ToList();

    public static Termin Create(Event @event, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        return new Termin(@event, date, startTime, endTime);
    }

    public void AddPlayers(List<Guid> userIds)
    {
        foreach (Guid userId in userIds)
        {
            _players.Add(Player.Create(userId, Id));
        }
    }
}