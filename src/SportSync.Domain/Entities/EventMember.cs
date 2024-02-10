using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class EventMember : Entity, ISoftDeletableEntity
{
    private EventMember(Guid userId, Guid eventId, bool isCreator = false)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        EventId = eventId;
        IsCreator = isCreator;
        IsAdmin = isCreator;
    }

    private EventMember()
    {
    }

    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsCreator { get; set; }

    public DateTime? DeletedOnUtc { get; set; }
    public bool Deleted { get; }

    public static EventMember Create(Guid userId, Guid eventId, bool isCreator = false)
    {
        return new EventMember(userId, eventId, isCreator);
    }
}