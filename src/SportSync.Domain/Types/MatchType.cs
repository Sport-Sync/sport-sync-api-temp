using System.Linq.Expressions;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Types;

public class MatchType
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string EventName { get; set; }
    public SportType SportType { get; set; }
    public MatchStatus Status { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public string Notes { get; set; }

    public static Expression<Func<Match, MatchType>> PropertySelector = x => new MatchType
    {
        Id = x.Id,
        Address = x.Address,
        Date = x.Date,
        StartTime = x.Date.Date + x.StartTime.TimeOfDay,
        EndTime = x.Date.Date + x.EndTime.TimeOfDay,
        SportType = x.SportType,
        NumberOfPlayersExpected = x.NumberOfPlayersExpected,
        Price = x.Price,
        Notes = x.Notes,
        EventName = x.EventName,
        Status = x.Status
    };

    public static MatchType FromMatch(Match match) => PropertySelector.Compile()(match);
}