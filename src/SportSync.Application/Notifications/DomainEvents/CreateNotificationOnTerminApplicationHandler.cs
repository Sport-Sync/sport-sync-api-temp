using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnTerminApplicationHandler : IDomainEventHandler<TerminApplicationSentDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ITerminRepository _terminRepository;

    public CreateNotificationOnTerminApplicationHandler(INotificationRepository notificationRepository, ITerminRepository terminRepository)
    {
        _notificationRepository = notificationRepository;
        _terminRepository = terminRepository;
    }

    public async Task Handle(TerminApplicationSentDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var terminApplication = domainEvent.TerminApplication;
        var termin = domainEvent.Termin;
        var admins = await _terminRepository.GetAdmins(terminApplication.TerminId, cancellationToken);

        var notifications = new List<Notification>();
        foreach (var admin in admins)
        {
            var notification = Notification.Create(
                admin.UserId, 
                NotificationTypeEnum.TerminApplicationReceived,
                NotificationContentData.Create(terminApplication.AppliedByUser.FullName, termin.EventName, termin.Date.ToShortDateString()),
                terminApplication.TerminId);

            notifications.Add(notification);
        }

        _notificationRepository.InsertRange(notifications);
    }
}