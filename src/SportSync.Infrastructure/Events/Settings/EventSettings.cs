namespace SportSync.Infrastructure.Events.Settings;

public class EventSettings
{
    public const string SettingsKey = "Event";

    public EventSettings()
    {

    }

    public int NumberOfTerminsToCreateInFuture { get; set; }
}