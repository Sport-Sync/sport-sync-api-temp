using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Services.Factories.Notification.Concrete;

public class CroatianNotificationContentProvider : INotificationContentProvider
{
    public string Content(NotificationTypeEnum type, NotificationContentData contentData)
    {
        return type switch
        {
            NotificationTypeEnum.FriendshipRequestReceived => $"{contentData[0]} vam je poslao zahtjev za prijateljstvom",
            NotificationTypeEnum.TerminApplicationReceived => $"{contentData[0]} se želi pridružiti na vašu utakmicu '{contentData[1]}' dana {contentData[2]}",
            NotificationTypeEnum.EventInvitationSent => $"{contentData[0]} vam je poslao zahtjev da se pridružite na termin '{contentData[1]}'",
            _ => throw new DomainException(DomainErrors.Notification.ContentNotImplemented)
        };
    }
}