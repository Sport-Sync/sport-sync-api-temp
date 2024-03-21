using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Notifications.Common;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.CompleteNotification;

public class CompleteNotificationRequestHandler : IRequestHandler<NotificationInput, Result>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteNotificationRequestHandler(
        INotificationRepository notificationRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IDateTime dateTime,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(NotificationInput request, CancellationToken cancellationToken)
    {
        var maybeNotification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);

        if (maybeNotification.HasNoValue)
        {
            return Result.Failure(DomainErrors.Notification.NotFound);
        }

        var notification = maybeNotification.Value;

        if (_userIdentifierProvider.UserId != notification.UserId)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        notification.Complete(_dateTime.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}