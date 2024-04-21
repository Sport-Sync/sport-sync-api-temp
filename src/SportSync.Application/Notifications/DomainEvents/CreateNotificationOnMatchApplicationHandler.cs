using SportSync.Application.Core.Extensions;
using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnMatchApplicationHandler : IDomainEventHandler<MatchApplicationSentDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMatchRepository _matchRepository;

    public CreateNotificationOnMatchApplicationHandler(INotificationRepository notificationRepository, IMatchRepository matchRepository)
    {
        _notificationRepository = notificationRepository;
        _matchRepository = matchRepository;
    }

    public async Task Handle(MatchApplicationSentDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var matchApplication = domainEvent.MatchApplication;
        var match = domainEvent.Match;
        var admins = await _matchRepository.GetAdmins(matchApplication.MatchId, cancellationToken);

        var notifications = new List<Notification>();
        foreach (var admin in admins)
        {
            var notification =
                Notification.Create(
                    admin.UserId,
                    NotificationTypeEnum.MatchApplicationReceived,
                    NotificationContentData.Create(matchApplication.AppliedByUser.FullName, match.EventName, match.Date.ToDateString()),
                    matchApplication.AppliedByUserId)
                .WithEntitySource(matchApplication.Id);

            notifications.Add(notification);
        }

        _notificationRepository.InsertRange(notifications);
    }
}