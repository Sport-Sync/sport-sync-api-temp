using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Entities;

public class Notification : AggregateRoot
{
    //private string _subjects;

    private Notification(Guid userId, NotificationTypeEnum type, Guid? resourceId = null)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");

        UserId = userId;
        ResourceId = resourceId;
        Type = type;
        //_subjects = string.Join(",", subjects);
    }

    private Notification()
    {
    }

    public Guid UserId { get; set; }
    public Guid? ResourceId { get; set; }
    public NotificationTypeEnum Type { get; set; }
    public DateTime? CompletedOnUtc { get; private set; }
    //public string Subjects { get; set; }

    public static Notification Create(Guid userId, NotificationTypeEnum type, Guid? resourceId = null)
    {
        return new Notification(userId, type, resourceId);
    }

    public void Complete(DateTime utcNow)
    {
        CompletedOnUtc = utcNow;
    }
}