namespace SportSync.Domain.Core.Utility;

public static class TimeZoneUtility
{
    private const string _timeZone = "Central European Standard Time";

    public static DateTimeOffset GetLocalDateTime(DateTime date, DateTimeOffset timeOffset)
    {
        var dateTimeOffset = new DateTimeOffset(date.Year, date.Month, date.Day, timeOffset.Hour, timeOffset.Minute, timeOffset.Second, timeOffset.Offset);

        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTimeOffset, _timeZone);
    }

    public static DateTimeOffset GetLocalDateTime(DateTime utcDateTime)
    {
        utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_timeZone);

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
    }
}