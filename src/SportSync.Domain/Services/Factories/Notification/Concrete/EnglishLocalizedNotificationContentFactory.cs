using SportSync.Domain.Enumerations;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Services.Factories.Notification.Concrete;

public class EnglishNotificationContentFactory : INotificationContentFactory
{
    public string Content(NotificationTypeEnum type, NotificationContentData contentData)
    {
        return type switch
        {
            NotificationTypeEnum.FriendshipRequestReceived => $"{contentData[0]} has sent you a friend request",
            NotificationTypeEnum.TerminApplicationReceived => $"{contentData[0]} wants to join to your termin '{contentData[1]}'",
            _ => string.Empty
        };
    }
}