namespace SportSync.Domain.Core.Utility;

public static class TimeZoneUtility
{
    private const string _timeZone = "Central European Standard Time";

    public static DateTimeOffset GetLocalDateTime(DateTime date, DateTimeOffset timeOffset)
    {
        var utcDateTime = DateTime.SpecifyKind(date.Date.Add(timeOffset.UtcDateTime.TimeOfDay), DateTimeKind.Utc);
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_timeZone);

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
    }

    public static DateTimeOffset GetLocalDateTime(DateTime utcDateTime)
    {
        utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_timeZone);

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
    }
}