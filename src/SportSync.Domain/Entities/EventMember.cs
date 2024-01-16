using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class EventMember : Entity
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

    public static EventMember Create(Guid userId, Guid eventId, bool isCreator = false)
    {
        return new EventMember(userId, eventId, isCreator);
    }
}