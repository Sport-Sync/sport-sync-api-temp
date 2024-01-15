using HotChocolate;
using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class EventMember : Entity, IAuditableEntity, ISoftDeletableEntity
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

    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsAdmin { get; set; }
    public bool IsCreator { get; set; }

    [GraphQLIgnore]
    public DateTime CreatedOnUtc { get; }
    [GraphQLIgnore]
    public DateTime? ModifiedOnUtc { get; }
    [GraphQLIgnore]
    public DateTime? DeletedOnUtc { get; }
    [GraphQLIgnore]
    public bool Deleted { get; }

    public static EventMember Create(Guid userId, Guid eventId, bool isCreator = false)
    {
        return new EventMember(userId, eventId, isCreator);
    }
}