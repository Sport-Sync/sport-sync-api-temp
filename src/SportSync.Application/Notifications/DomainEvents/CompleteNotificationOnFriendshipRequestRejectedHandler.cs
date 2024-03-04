using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.DomainEvents;

public class CompleteNotificationOnFriendshipRequestRejectedHandler : IDomainEventHandler<FriendshipRequestRejectedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTime _dateTime;

    public CompleteNotificationOnFriendshipRequestRejectedHandler(INotificationRepository notificationRepository, IDateTime dateTime)
    {
        _notificationRepository = notificationRepository;
        _dateTime = dateTime;
    }

    public async Task Handle(FriendshipRequestRejectedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var notification = (await _notificationRepository.GetByResourceIdAndType(
            domainEvent.FriendshipRequest.Id,
            NotificationTypeEnum.FriendshipRequestReceived,
            cancellationToken)).SingleOrDefault();

        if (notification == null)
        {
            return;
        }

        notification.Complete(_dateTime.UtcNow);
    }
}