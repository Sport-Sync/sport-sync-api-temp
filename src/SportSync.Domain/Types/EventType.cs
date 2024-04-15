using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Types;

public class EventType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public SportTypeEnum SportType { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayers { get; set; }
    public string Notes { get; set; }

    public static EventType FromEvent(Event @event) => new()
    {
        Id = @event.Id,
        Name = @event.Name,
        NumberOfPlayers = @event.NumberOfPlayers,
        Notes = @event.Notes,
        Address = @event.Address,
        Price = @event.Price,
        SportType = @event.SportType
    };
}