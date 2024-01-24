using SportSync.Domain.Entities;

namespace SportSync.Application.Core.Abstractions.Notifications;

public interface IPushNotificationService
{
    Task NotifyAboutEventCreated(Event @event);
}