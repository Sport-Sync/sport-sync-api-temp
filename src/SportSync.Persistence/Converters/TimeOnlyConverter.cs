using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SportSync.Persistence.Converters;

public class TimeOnlyConverter : ValueConverter<DateTime, TimeSpan>
{
    public TimeOnlyConverter() : base(
        dateTime => dateTime.TimeOfDay,
        timeSpan => new DateTime() + timeSpan)
    { }
}