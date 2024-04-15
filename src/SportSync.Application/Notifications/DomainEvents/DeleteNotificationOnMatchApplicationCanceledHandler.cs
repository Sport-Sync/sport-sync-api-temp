using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.DomainEvents;

public class DeleteNotificationOnMatchApplicationCanceledHandler : IDomainEventHandler<MatchApplicationCanceledDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;

    public DeleteNotificationOnMatchApplicationCanceledHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(MatchApplicationCanceledDomainEvent domainEvent, CancellationToken cancellationToken)
    {

        var notifications = await _notificationRepository.GetForEntitySource(
            domainEvent.MatchApplication.Id,
            NotificationTypeEnum.MatchApplicationReceived,
            cancellationToken);

        foreach (var notification in notifications)
        {
            _notificationRepository.Remove(notification);
        }
    }
}