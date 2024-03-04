using System.Linq.Expressions;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Types;

public class NotificationType
{
    public Guid Id { get; set; }
    public Guid? ResourceId { get; set; }
    public NotificationTypeEnum Type { get; set; }

    public static Expression<Func<Notification, NotificationType>> PropertySelector = x => new NotificationType
    {
        Id = x.Id,
        Type = x.Type,
        ResourceId = x.ResourceId
    };
}