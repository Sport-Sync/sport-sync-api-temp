using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class MatchAnnouncement : Entity
{
    public MatchAnnouncement(Match match, Guid userId, MatchAnnouncementType announcementType)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");
        Ensure.NotNull(match, "The match is required.", nameof(match));
        Ensure.NotEmpty(match.Id, "The match identifier is required.", $"{nameof(match)}{nameof(match.Id)}");

        UserId = userId;
        MatchId = match.Id;
        AnnouncementType = announcementType;
    }

    private MatchAnnouncement()
    {
    }

    public Guid UserId { get; set; }

    public Guid MatchId { get; set; }

    public MatchAnnouncementType AnnouncementType { get; set; }
}