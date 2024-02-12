using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class TerminAnnouncement : Entity
{
    public TerminAnnouncement(Termin termin, Guid userId, TerminAnnouncementType announcementType)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");
        Ensure.NotNull(termin, "The termin is required.", nameof(termin));
        Ensure.NotEmpty(termin.Id, "The termin identifier is required.", $"{nameof(termin)}{nameof(termin.Id)}");

        UserId = userId;
        TerminId = termin.Id;
        AnnouncementType = announcementType;
    }

    private TerminAnnouncement()
    {
    }

    public Guid UserId { get; set; }

    public Guid TerminId { get; set; }

    public TerminAnnouncementType AnnouncementType { get; set; }
}