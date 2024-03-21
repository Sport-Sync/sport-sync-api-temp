using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Services.Factories.Notification.Concrete;

public class EnglishNotificationContentProvider : INotificationContentProvider
{
    public string Content(NotificationTypeEnum type, NotificationContentData contentData)
    {
        return type switch
        {
            NotificationTypeEnum.FriendshipRequestReceived => $"{contentData[0]} has sent you a friend request",
            NotificationTypeEnum.MatchApplicationReceived => $"{contentData[0]} wants to join to your match '{contentData[1]}' on day {contentData[2]}",
            NotificationTypeEnum.EventInvitationReceived => $"{contentData[0]} has sent you a request to join their event '{contentData[1]}'",
            NotificationTypeEnum.EventInvitationAccepted => $"{contentData[0]} has accepted your invitation to event '{contentData[1]}'",
            NotificationTypeEnum.MemberJoinedEvent => $"{contentData[0]} is the new member on your event '{contentData[1]}'",
            NotificationTypeEnum.EventInvitationRejected => $"{contentData[0]} has rejected your invitation to join event '{contentData[1]}'",

            _ => throw new DomainException(DomainErrors.Notification.ContentNotImplemented)
        };
    }
}