using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnEventInvitationRejectedHandler : IDomainEventHandler<EventInvitationRejectedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationOnEventInvitationRejectedHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public Task Handle(EventInvitationRejectedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventInvitation = domainEvent.Invitation;
        var @event = domainEvent.Event;
        var user = domainEvent.User;

        var notification = Notification.Create(
            eventInvitation.SentByUserId,
            NotificationTypeEnum.EventInvitationRejected,
            NotificationContentData.Create(user.FullName, @event.Name),
            @event.Id);

        _notificationRepository.Insert(notification);

        return Task.CompletedTask;
    }
}