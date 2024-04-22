using SportSync.Application.Core.Extensions;
using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnFriendAddedToMatchHandler : IDomainEventHandler<FriendAddedToMatchDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationOnFriendAddedToMatchHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public Task Handle(FriendAddedToMatchDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var addedByUser = domainEvent.AddedByUser;
        var friendId = domainEvent.FriendId;
        var match = domainEvent.Match;
        
        var notification = Notification.Create(
            friendId,
            NotificationTypeEnum.AddedToMatchByFriend,
            NotificationContentData.Create(addedByUser.FullName, match.EventName, match.Date.ToDateString()),
            match.Id);

        _notificationRepository.Insert(notification);

        return Task.CompletedTask;
    }
}