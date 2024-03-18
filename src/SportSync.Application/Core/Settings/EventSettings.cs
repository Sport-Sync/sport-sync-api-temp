namespace SportSync.Application.Core.Settings;

public class EventSettings
{
    public const string SettingsKey = "Event";

    public EventSettings()
    {

    }

    public int NumberOfMatchesToCreateInFuture { get; set; }
}