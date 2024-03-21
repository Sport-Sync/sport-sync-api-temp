using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Notifications.Common;

public class NotificationInput : IRequest<Result>
{
    public Guid NotificationId { get; set; }
}