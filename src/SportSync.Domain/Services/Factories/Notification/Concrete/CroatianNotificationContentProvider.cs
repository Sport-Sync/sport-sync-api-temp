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
            NotificationTypeEnum.MatchApplicationReceived => $"{contentData[0]} se želi pridružiti na vašu utakmicu '{contentData[1]}' dana {contentData[2]}",
            NotificationTypeEnum.EventInvitationReceived => $"{contentData[0]} vam je poslao zahtjev da se pridružite na termin '{contentData[1]}'",
            NotificationTypeEnum.EventInvitationAccepted => $"{contentData[0]} je prihvatio vaš poziv na termin '{contentData[1]}'",
            NotificationTypeEnum.MemberJoinedEvent => $"{contentData[0]} je upravo postao novi član vašeg termina '{contentData[1]}'",
            NotificationTypeEnum.EventInvitationRejected => $"{contentData[0]} je odbio vaš poziv za pridruživanje na termin '{contentData[1]}'",
            NotificationTypeEnum.MatchAnnouncedByFriend => $"{contentData[0]} traži igrače za utakmicu '{contentData[1]}' dana {contentData[2]}",
            NotificationTypeEnum.MatchApplicationAccepted => $"{contentData[0]} je prihvatio vašu prijavu na utakmicu '{contentData[1]}' dana {contentData[2]}",

            _ => throw new DomainException(DomainErrors.Notification.ContentNotImplemented)
        };
    }
}