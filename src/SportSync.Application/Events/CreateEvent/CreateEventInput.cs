using SportSync.Application.Core.Abstractions.Common;
using SportSync.Domain.Enumerations;

namespace SportSync.Application.Events.CreateEvent;

public class CreateEventInput : IRequest<Guid>
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
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool RepeatWeekly { get; set; }
}
