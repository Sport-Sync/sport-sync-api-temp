namespace SportSync.Application.Core.Extensions;

public static class DateTimeExtensions
{
    public static string ToDateString(this DateTime dateTime)
    {
        return dateTime.ToString("dd.MM.yyyy.");
    }
}