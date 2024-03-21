using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnEventInvitationAcceptedHandler : IDomainEventHandler<EventInvitationAcceptedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationOnEventInvitationAcceptedHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public Task Handle(EventInvitationAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventInvitation = domainEvent.Invitation;
        var @event = domainEvent.Event;
        var user = domainEvent.User;
        
        var notifications = new List<Notification>
        {
            Notification.Create(
                eventInvitation.SentByUserId,
                NotificationTypeEnum.EventInvitationAccepted,
                NotificationContentData.Create(user.FullName, @event.Name),
                @event.Id)
        };

        var membersToNotify = @event.Members
            .Where(m => m.UserId != eventInvitation.SentByUserId && m.UserId != user.Id);

        foreach (var member in membersToNotify)
        {
            var notification = Notification.Create(
                member.UserId,
                NotificationTypeEnum.MemberJoinedEvent,
                NotificationContentData.Create(user.FullName, @event.Name),
                @event.Id);

            notifications.Add(notification);
        }

        _notificationRepository.InsertRange(notifications);

        return Task.CompletedTask;
    }
}