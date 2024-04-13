using SportSync.Domain.Enumerations;

namespace SportSync.Application.Events.CreateEvent;

public class CreateEventInput : IRequest<Guid>
{
    public List<Guid> MemberIds { get; set; }
    public string Name { get; set; }
    public SportTypeEnum SportType { get; set; }
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
    public bool RepeatWeekly { get; set; }
}
