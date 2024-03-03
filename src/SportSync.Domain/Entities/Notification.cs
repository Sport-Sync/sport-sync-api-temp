using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class Notification : AggregateRoot
{
    private Notification(Guid userId, NotificationType type, Guid? resourceId = null)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");

        UserId = userId;
        ResourceId = resourceId;
        Type = type;
    }

    private Notification()
    {
    }

    public Guid UserId { get; set; }
    public Guid? ResourceId { get; set; }
    public NotificationType Type { get; set; }
    public DateTime? CompletedOnUtc { get; private set; }

    public static Notification Create(Guid userId, NotificationType type, Guid? resourceId = null)
    {
        return new Notification(userId, type, resourceId);
    }

    public void Complete(DateTime utcNow)
    {
        CompletedOnUtc = utcNow;
    }
}