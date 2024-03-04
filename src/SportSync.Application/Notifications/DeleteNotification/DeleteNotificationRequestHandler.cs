using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Notifications.DeleteNotification;

public class DeleteNotificationRequestHandler : IRequestHandler<DeleteNotificationInput, Result>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNotificationRequestHandler(INotificationRepository notificationRepository, IUserIdentifierProvider userIdentifierProvider, IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteNotificationInput request, CancellationToken cancellationToken)
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

        _notificationRepository.Remove(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(notification);
    }
}