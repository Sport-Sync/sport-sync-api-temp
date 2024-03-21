using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Entities;

public class Notification : AggregateRoot
{
    private Notification(Guid userId, NotificationTypeEnum type, NotificationContentData contentData, Guid? resourceId = null)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");

        UserId = userId;
        ResourceId = resourceId;
        Type = type;
        ContentData = contentData;
    }

    private Notification()
    {
    }

    public Guid UserId { get; set; }
    public Guid? ResourceId { get; set; }
    public Guid? EntitySourceId { get; set; }
    public NotificationTypeEnum Type { get; set; }
    public DateTime? CompletedOnUtc { get; private set; }
    public NotificationContentData ContentData { get; set; }

    public bool Completed => CompletedOnUtc != null;

    public static Notification Create(Guid userId, NotificationTypeEnum type, NotificationContentData details, Guid? resourceId = null)
    {
        return new Notification(userId, type, details, resourceId);
    }

    public Notification WithEntitySource(Guid entitySourceId)
    {
        EntitySourceId = entitySourceId;
        return this;
    }

    public void Complete(DateTime utcNow)
    {
        if (Completed)
        {
            return;
        }

        CompletedOnUtc = utcNow;
    }
}