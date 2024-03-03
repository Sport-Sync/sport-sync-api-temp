using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnFriendshipRequestSentDomainEventHandler : IDomainEventHandler<FriendshipRequestSentDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationOnFriendshipRequestSentDomainEventHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public Task Handle(FriendshipRequestSentDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: use factory (think about how to use localization)
        var notification =
            Notification.Create(domainEvent.FriendshipRequest.FriendId, NotificationType.FriendshipRequestReceived,
                NotificationCommands.AcceptReject, domainEvent.FriendshipRequest.Id);

        _notificationRepository.Insert(notification);

        return Task.CompletedTask;
    }
}