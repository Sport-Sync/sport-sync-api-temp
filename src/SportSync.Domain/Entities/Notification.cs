using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Entities;

public class Notification : AggregateRoot
{
    private Notification(Guid userId, NotificationTypeEnum type, NotificationDetails details, Guid? resourceId = null)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");

        UserId = userId;
        ResourceId = resourceId;
        Type = type;
        Details = details;
    }

    private Notification()
    {
    }

    public Guid UserId { get; set; }
    public Guid? ResourceId { get; set; }
    public NotificationTypeEnum Type { get; set; }
    public DateTime? CompletedOnUtc { get; private set; }
    public NotificationDetails Details { get; set; }

    public static Notification Create(Guid userId, NotificationTypeEnum type, Guid? resourceId = null, NotificationDetails details = null)
    {
        details ??= NotificationDetails.Create();
        return new Notification(userId, type, details, resourceId);
    }

    public void Complete(DateTime utcNow)
    {
        CompletedOnUtc = utcNow;
    }
}