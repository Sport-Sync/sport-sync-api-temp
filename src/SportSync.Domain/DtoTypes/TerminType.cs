using SportSync.Domain.Enumerations;

namespace SportSync.Domain.DtoTypes;

public class TerminType
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTimeUtc { get; set; }
    public TimeOnly EndTimeUtc { get; set; }
    public string EventName { get; set; }
    public SportType SportType { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public int NumberOfPlayers { get; set; }
    public string Notes { get; set; }
}