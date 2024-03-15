using System.Linq.Expressions;
using HotChocolate;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Types;

public class NotificationType
{
    public Guid NotificationId { get; set; }
    public Guid? ResourceId { get; set; }
    public NotificationTypeEnum Type { get; set; }
    public string Content { get; set; }
    [GraphQLIgnore]
    public NotificationContentData ContentData { get; set; }
    public bool Completed { get; set; }

    public static Expression<Func<Notification, NotificationType>> PropertySelector = x => new NotificationType
    {
        NotificationId = x.Id,
        Type = x.Type,
        ResourceId = x.ResourceId,
        ContentData = x.ContentData,
        Completed = x.CompletedOnUtc != null
    };
}