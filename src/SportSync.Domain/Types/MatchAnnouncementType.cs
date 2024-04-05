using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Types;

public class MatchAnnouncementType
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string EventName { get; set; }
    public SportType SportType { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayersLimit { get; set; }
    public int NumberOfPlayersAccepted { get; set; }
    public string Description { get; set; }
    public bool ApplicationAlreadySent { get; set; }
    public bool UserIsPlayer { get; set; }

    public MatchAnnouncementType(MatchAnnouncement matchAnnouncement, Match match, bool applicationAlreadySent, bool userIsPlayer)
    {
        Id = matchAnnouncement.Id;
        MatchId = matchAnnouncement.MatchId;
        Address = match.Address;
        Date = match.Date;
        StartTime = match.StartTime.DateTime.ToUniversalTime();
        EndTime = match.EndTime.DateTime.ToUniversalTime();
        SportType = match.SportType;
        NumberOfPlayersLimit = matchAnnouncement.NumberOfPlayersLimit;
        NumberOfPlayersAccepted = matchAnnouncement.NumberOfPlayersAccepted;
        Price = match.Price;
        Description = matchAnnouncement.Description;
        EventName = match.EventName;
        ApplicationAlreadySent = applicationAlreadySent;
        UserIsPlayer = userIsPlayer;
    }
}