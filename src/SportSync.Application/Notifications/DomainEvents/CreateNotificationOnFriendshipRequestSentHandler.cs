using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnFriendshipRequestSentHandler : IDomainEventHandler<FriendshipRequestSentDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationOnFriendshipRequestSentHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public Task Handle(FriendshipRequestSentDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var friendshipRequest = domainEvent.FriendshipRequest;
        var notification = Notification.Create(friendshipRequest.FriendId, NotificationTypeEnum.FriendshipRequestReceived, friendshipRequest.Id);

        _notificationRepository.Insert(notification);

        return Task.CompletedTask;
    }
}