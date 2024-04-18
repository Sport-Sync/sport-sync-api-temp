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
    public SportTypeEnum SportType { get; set; }
    public MatchAnnouncementTypeEnum? TypeOfAnnouncement { get; set; }
    public bool IsAnnounced { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersExpected { get; set; }
    public string Notes { get; set; }
    public MatchStatusEnum Status { get; set; }

    public static MatchType FromMatch(Match match) => new()
    {
        Id = match.Id,
        Address = match.Address,
        Date = match.Date,
        StartTime = match.StartTime.UtcDateTime,
        EndTime = match.EndTime.UtcDateTime,
        SportType = match.SportType,
        NumberOfPlayersExpected = match.NumberOfPlayersExpected,
        Price = match.Price,
        Notes = match.Notes,
        EventName = match.EventName,
        TypeOfAnnouncement = match.Announcement?.AnnouncementType,
        IsAnnounced = match.Announced,
        Status = match.Status
    };
}