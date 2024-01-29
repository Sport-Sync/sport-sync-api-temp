namespace SportSync.Api.Tests.Extensions;

public static class DateTimeExtensions
{
    public static string ToIsoString(this DateTime dateTime)
    {
        return dateTime.ToString("s") + "Z";
    }
}