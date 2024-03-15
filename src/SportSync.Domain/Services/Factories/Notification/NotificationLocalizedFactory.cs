using SportSync.Domain.Services.Factories.Notification.Concrete;

namespace SportSync.Domain.Services.Factories.Notification;

public static class NotificationLocalizedFactory
{
    public static INotificationContentFactory GetLocalizedFactory(string language)
    {
        return language switch
        {
            "En" => new EnglishNotificationContentFactory(),
            "Hr" => new CroatianNotificationContentFactory(),
            _ => new CroatianNotificationContentFactory()
        };
    }
}