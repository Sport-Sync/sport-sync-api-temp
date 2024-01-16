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
    public List<EventTimeInput> EventTime { get; set; }
}

public class EventTimeInput
{
    public DateTime Date { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public bool RepeatWeekly { get; set; }
}