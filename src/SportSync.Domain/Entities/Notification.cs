using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Entities;

public class Notification : AggregateRoot
{
    private Notification(Guid userId, NotificationType type, NotificationActions actions)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");
        
        UserId = userId;
        Type = type;
        Actions = actions ?? NotificationActions.None;
    }

    private Notification()
    {
    }

    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationActions Actions { get; set; }

    public static Notification Create(Guid userId, NotificationType type, NotificationActions actions)
    {
        return new Notification(userId, type, actions);
    }
}