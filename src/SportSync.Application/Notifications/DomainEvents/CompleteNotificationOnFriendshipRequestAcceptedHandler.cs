using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.DomainEvents;

public class CompleteNotificationOnFriendshipRequestAcceptedHandler : IDomainEventHandler<FriendshipRequestAcceptedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTime _dateTime;

    public CompleteNotificationOnFriendshipRequestAcceptedHandler(INotificationRepository notificationRepository, IDateTime dateTime)
    {
        _notificationRepository = notificationRepository;
        _dateTime = dateTime;
    }

    public async Task Handle(FriendshipRequestAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var notification = (await _notificationRepository.GetByResourceIdAndType(
            domainEvent.FriendshipRequest.Id, 
            NotificationType.FriendshipRequestReceived, 
            cancellationToken)).SingleOrDefault();

        if (notification == null)
        {
            return;
        }

        notification.Complete(_dateTime.UtcNow);
    }
}