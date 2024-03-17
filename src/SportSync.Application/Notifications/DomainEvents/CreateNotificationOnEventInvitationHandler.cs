using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnEventInvitationHandler : IDomainEventHandler<EventInvitationSentDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationOnEventInvitationHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public Task Handle(EventInvitationSentDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventInvitation = domainEvent.EventInvitation;

        var notification = Notification.Create(
            eventInvitation.SentToUserId,
            NotificationTypeEnum.EventInvitationSent,
            NotificationContentData.Create(eventInvitation.SentByUser.FullName, domainEvent.Event.Name),
            eventInvitation.EventId);

        _notificationRepository.Insert(notification);

        return Task.CompletedTask;
    }
}