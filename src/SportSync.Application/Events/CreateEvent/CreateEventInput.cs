using HotChocolate;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Domain.Enumerations;

namespace SportSync.Application.Events.CreateEvent;

public class CreateEventInput : IInput<Guid>
{
    public List<Guid> MemberIds { get; set; }
    public string Name { get; set; }
    public SportType SportType { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int NumberOfPlayers { get; set; }
    public string? Notes { get; set; }
    public List<TimeInput> EventTime { get; set; }
}

public class TimeInput
{
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime StartDate { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }

    [GraphQLIgnore]
    public TimeOnly StartTimeUtc => TimeOnly.FromDateTime(StartTime.UtcDateTime);

    [GraphQLIgnore]
    public TimeOnly EndTimeUtc => TimeOnly.FromDateTime(EndTime.UtcDateTime);

    public bool RepeatWeekly { get; set; }
}
