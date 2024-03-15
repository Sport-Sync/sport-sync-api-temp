using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Services.Factories.Notification.Concrete;

public class CroatianNotificationContentFactory : INotificationContentFactory
{
    public string Content(NotificationTypeEnum type, NotificationContentData contentData)
    {
        return type switch
        {
            NotificationTypeEnum.FriendshipRequestReceived => $"{contentData[0]} vam je poslao zahtjev za prijateljstvom",
            NotificationTypeEnum.TerminApplicationReceived => $"{contentData[0]} se želi pridružiti na vaš termin '{contentData[1]}'",
            _ => throw new DomainException(DomainErrors.Notification.ContentNotImplemented)
        };
    }
}