using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.DomainEvents;

public class CompleteNotificationOnEventInvitationAcceptedHandler : IDomainEventHandler<EventInvitationAcceptedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTime _dateTime;

    public CompleteNotificationOnEventInvitationAcceptedHandler(INotificationRepository notificationRepository, IDateTime dateTime)
    {
        _notificationRepository = notificationRepository;
        _dateTime = dateTime;
    }

    public async Task Handle(EventInvitationAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var notification = (await _notificationRepository.GetForEntitySource(
            domainEvent.Invitation.Id,
            NotificationTypeEnum.EventInvitationReceived,
            cancellationToken)).SingleOrDefault();

        if (notification == null)
        {
            return;
        }

        notification.Complete(_dateTime.UtcNow);
    }
}