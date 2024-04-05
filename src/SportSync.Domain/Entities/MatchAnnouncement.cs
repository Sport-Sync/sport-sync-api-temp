using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class MatchAnnouncement : Entity, ISoftDeletableEntity
{
    public MatchAnnouncement(Match match, Guid userId, MatchAnnouncementType announcementType, int numberOfPlayersLimit, string description)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");
        Ensure.NotNull(match, "The match is required.", nameof(match));
        Ensure.NotEmpty(match.Id, "The match identifier is required.", $"{nameof(match)}{nameof(match.Id)}");

        UserId = userId;
        MatchId = match.Id;
        AnnouncementType = announcementType;
        NumberOfPlayersLimit = numberOfPlayersLimit;
        Description = description;
    }

    private MatchAnnouncement()
    {
    }

    public Guid UserId { get; set; }
    public Guid MatchId { get; set; }
    public MatchAnnouncementType AnnouncementType { get; set; }
    public int NumberOfPlayersLimit { get; set; }
    public int NumberOfPlayersAccepted { get; set; }
    public string Description { get; set; }
    public DateTime? DeletedOnUtc { get; private set; }
    public bool Deleted { get; private set; }

    public void Delete()
    {
        DeletedOnUtc = DateTime.UtcNow;
        Deleted = true;
    }
}