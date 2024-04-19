using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Types;

public class MatchAnnouncementType
{
    public Guid MatchId { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string EventName { get; set; }
    public SportTypeEnum SportType { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int PlayerLimit { get; set; }
    public int AcceptedPlayersCount { get; set; }
    public string Description { get; set; }
    public MatchAnnouncementTypeEnum TypeOfAnnouncement { get; set; }
    public bool CurrentUserAlreadyApplied { get; set; }
    public bool CurrentUserIsPlayer { get; set; }

    public MatchAnnouncementType(Match match, bool currentUserAlreadyApplied, bool currentUserIsPlayer)
    {
        MatchId = match.Announcement.MatchId;
        Address = match.Address;
        Date = match.Date;
        StartTime = match.StartTime.UtcDateTime;
        EndTime = match.EndTime.UtcDateTime;
        SportType = match.SportType;
        PlayerLimit = match.Announcement.PlayerLimit;
        AcceptedPlayersCount = match.Announcement.AcceptedPlayersCount;
        Price = match.Price;
        Description = match.Announcement.Description;
        EventName = match.EventName;
        CurrentUserAlreadyApplied = currentUserAlreadyApplied;
        CurrentUserIsPlayer = currentUserIsPlayer;
        TypeOfAnnouncement = match.Announcement.AnnouncementType;
    }

    public MatchAnnouncementType()
    {
        
    }
}