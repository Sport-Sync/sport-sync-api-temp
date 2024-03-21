using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.DomainEvents;

public class CompleteNotificationOnMatchApplicationAcceptedHandler : IDomainEventHandler<MatchApplicationAcceptedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDateTime _dateTime;

    public CompleteNotificationOnMatchApplicationAcceptedHandler(INotificationRepository notificationRepository, IDateTime dateTime)
    {
        _notificationRepository = notificationRepository;
        _dateTime = dateTime;
    }

    public async Task Handle(MatchApplicationAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (!domainEvent.MatchApplication.CompletedByUserId.HasValue)
        {
            return;
        }

        var notification = (await _notificationRepository.GetForEntitySource(
            domainEvent.MatchApplication.Id,
            NotificationTypeEnum.MatchApplicationReceived,
            domainEvent.MatchApplication.CompletedByUserId.Value,
            cancellationToken)).SingleOrDefault();

        if (notification == null)
        {
            return;
        }

        notification.Complete(_dateTime.UtcNow);
    }
}