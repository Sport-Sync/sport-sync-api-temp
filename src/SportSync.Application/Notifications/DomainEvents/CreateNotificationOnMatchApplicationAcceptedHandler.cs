using SportSync.Application.Core.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Events;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Notifications.DomainEvents;

public class CreateNotificationOnMatchApplicationAcceptedHandler : IDomainEventHandler<MatchApplicationAcceptedDomainEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMatchRepository _matchRepository;

    public CreateNotificationOnMatchApplicationAcceptedHandler(INotificationRepository notificationRepository, IMatchRepository matchRepository)
    {
        _notificationRepository = notificationRepository;
        _matchRepository = matchRepository;
    }

    public async Task Handle(MatchApplicationAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (!domainEvent.MatchApplication.CompletedByUserId.HasValue)
        {
            return;
        }

        var matchApplication = domainEvent.MatchApplication;
        var acceptedByUser = domainEvent.MatchApplication.CompletedByUser;
        var maybeMatch = await _matchRepository.GetByIdAsync(matchApplication.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            throw new DomainException(DomainErrors.Match.NotFound);
        }

        var match = maybeMatch.Value;

        var notification =
            Notification.Create(
                    matchApplication.AppliedByUserId,
                    NotificationTypeEnum.MatchApplicationAccepted,
                    NotificationContentData.Create(acceptedByUser.FullName, match.EventName, match.Date.ToDateString()),
                    match.Id);

        _notificationRepository.Insert(notification);
    }
}