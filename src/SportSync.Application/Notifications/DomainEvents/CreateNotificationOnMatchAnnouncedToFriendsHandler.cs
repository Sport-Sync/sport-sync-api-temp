using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnMatchAnnouncedToFriendsHandler : IDomainEventHandler<MatchAnnouncedToFriendsDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationOnMatchAnnouncedToFriendsHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public Task Handle(MatchAnnouncedToFriendsDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var match = domainEvent.Match;
        var user = domainEvent.Announcer;

        var notifications = new List<Notification>();

        var friendsToNotify = user.Friends.Where(friendId => match.IsPlayer(friendId) == false);

        foreach (var friendId in friendsToNotify)
        {
            var notification = Notification.Create(
                friendId,
                NotificationTypeEnum.MatchAnnouncedByFriend,
                NotificationContentData.Create(user.FullName, match.EventName, match.Date.ToShortDateString()),
                match.Id);

            notifications.Add(notification);
        }

        _notificationRepository.InsertRange(notifications);

        return Task.CompletedTask;
    }
}