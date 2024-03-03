using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Entities;

public class Notification : AggregateRoot
{
    private Notification(Guid userId, NotificationType type, NotificationCommands commands, Guid? resourceId = null)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(userId, "The user identifier is required.", $"{nameof(userId)}");
        
        UserId = userId;
        ResourceId = resourceId;
        Type = type;
        Commands = commands ?? NotificationCommands.None;
    }

    private Notification()
    {
    }

    public Guid UserId { get; set; }
    public Guid? ResourceId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationCommands Commands { get; set; }
    public DateTime? CompletedOnUtc { get; private set; }
    public string CompletedWithCommand { get; set; }

    public static Notification Create(Guid userId, NotificationType type, NotificationCommands commands, Guid? resourceId = null)
    {
        return new Notification(userId, type, commands, resourceId);
    }
}