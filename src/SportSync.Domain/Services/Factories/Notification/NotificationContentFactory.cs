using SportSync.Domain.Core.Constants;
using SportSync.Domain.Services.Factories.Notification.Concrete;

namespace SportSync.Domain.Services.Factories.Notification;

public static class NotificationContentFactory
{
    public static INotificationContentProvider GetContentProvider(string language)
    {
        return language switch
        {
            LocalizationConstants.English => new EnglishNotificationContentProvider(),
            LocalizationConstants.Croatian => new CroatianNotificationContentProvider(),
            _ => new CroatianNotificationContentProvider()
        };
    }
}