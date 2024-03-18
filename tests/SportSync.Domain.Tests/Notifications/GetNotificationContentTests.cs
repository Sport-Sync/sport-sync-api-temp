using System;
using System.Linq;
using FluentAssertions;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Services.Factories.Notification;
using SportSync.Domain.ValueObjects;
using Xunit;

namespace SportSync.Domain.Tests.Notifications;

public class GetNotificationContentTests
{
    [Theory]
    [InlineData("Hr")]
    [InlineData("En")]
    public void GetNotificationContent_ShouldBeImplemented_ForAllNotificationTypes(string language)
    {
        var notificationTypes = Enum.GetValues(typeof(NotificationTypeEnum)).Cast<NotificationTypeEnum>().ToList();

        var contentProvider = NotificationContentFactory.GetContentProvider(language);
        foreach (var notificationType in notificationTypes)
        {
            var content = contentProvider.Content(notificationType, NotificationContentData.None);
            content.Should().NotBeEmpty();
        }
    }
}