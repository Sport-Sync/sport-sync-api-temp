using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class MatchAnnouncement : Entity, ISoftDeletableEntity
{
    public MatchAnnouncement(Match match, Guid userId, MatchAnnouncementTypeEnum announcementType, int playerLimit, string description)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");
        Ensure.NotNull(match, "The match is required.", nameof(match));
        Ensure.NotEmpty(match.Id, "The match identifier is required.", $"{nameof(match)}{nameof(match.Id)}");

        UserId = userId;
        MatchId = match.Id;
        AnnouncementType = announcementType;
        PlayerLimit = playerLimit;
        Description = description;
    }

    private MatchAnnouncement()
    {
    }

    public Guid UserId { get; set; }
    public Guid MatchId { get; set; }
    public MatchAnnouncementTypeEnum AnnouncementType { get; set; }
    public int PlayerLimit { get; set; }
    public int AcceptedPlayersCount { get; set; }
    public string Description { get; set; }
    public DateTime? DeletedOnUtc { get; private set; }
    public bool Deleted { get; private set; }

    public void Update(Guid userId, MatchAnnouncementTypeEnum announcementType, int playerLimit, string description)
    {
        // Only way we can update is from private -> public
        if (AnnouncementType != MatchAnnouncementTypeEnum.FriendList)
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.AlreadyPubliclyAnnounced);
        }

        if (announcementType != MatchAnnouncementTypeEnum.Public)
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.AlreadyAnnounced);
        }

        if (playerLimit <= AcceptedPlayersCount)
        {
            throw new DomainException(DomainErrors.MatchAnnouncement.PlayerLimitLessThanAlreadyAccepted);
        }

        UserId = userId;
        AnnouncementType = announcementType;
        PlayerLimit = playerLimit;
        Description = description;
    }

    public void Delete()
    {
        DeletedOnUtc = DateTime.UtcNow;
        Deleted = true;
    }
}