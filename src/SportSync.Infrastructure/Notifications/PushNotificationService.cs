using FirebaseAdmin.Messaging;
using SportSync.Application.Core.Abstractions.Notifications;
using SportSync.Domain.Entities;

namespace SportSync.Infrastructure.Notifications;

public class PushNotificationService : IPushNotificationService
{
    public async Task NotifyAboutEventCreated(Event @event)
    {
        var creator = @event.Members.SingleOrDefault(x => x.IsCreator);

        var memberIds = @event.Members.Where(x => !x.IsCreator);
        var messages = new List<Message>();

        foreach (var memberId in memberIds)
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "X je kreirao novi termin",
                    Body = "Novi temrin je kreiran ponedjeljkom"
                },
                Data = new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" },
                },
                Token = "device_registration_token" // Replace with the actual device token of the target device
            };

            messages.Add(message);
        }


        var result = await FirebaseMessaging.DefaultInstance.SendAllAsync(messages);
        if (result.FailureCount == 0)
        {
            // Message was sent successfully
        }
        else
        {
            // There was an error sending the message
            throw new Exception("Error sending the message.");
        }
    }
}