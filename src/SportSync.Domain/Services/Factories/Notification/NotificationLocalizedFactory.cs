using SportSync.Domain.Core.Constants;
using SportSync.Domain.Services.Factories.Notification.Concrete;

namespace SportSync.Domain.Services.Factories.Notification;

public static class NotificationLocalizedFactory
{
    public static INotificationContentFactory GetLocalizedFactory(string language)
    {
        return language switch
        {
            LocalizationConstants.English => new EnglishNotificationContentFactory(),
            LocalizationConstants.Croatian => new CroatianNotificationContentFactory(),
            _ => new CroatianNotificationContentFactory()
        };
    }
}