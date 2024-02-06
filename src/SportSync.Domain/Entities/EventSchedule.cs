using SportSync.Domain.Core.Abstractions;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class EventSchedule : Entity, ISoftDeletableEntity
{
    private EventSchedule(DayOfWeek dayOfWeek, DateTime startDate, DateTime startTime, DateTime endTime, bool repeatWeekly)
        : base(Guid.NewGuid())
    {
        DayOfWeek = dayOfWeek;
        StartDate = startDate;
        StartTime = startTime;
        EndTime = endTime;
        RepeatWeekly = repeatWeekly;
    }

    private EventSchedule()
    {
    }

    public Guid EventId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool RepeatWeekly { get; set; }
    public Event Event { get; set; }

    public DateTime? DeletedOnUtc { get; set; }
    public bool Deleted { get; }

    public static EventSchedule Create(DayOfWeek dayOfWeek, DateTime startDate, DateTime startTime, DateTime endTime, bool repeatWeekly)
    {
        return new EventSchedule(dayOfWeek, startDate, startTime, endTime, repeatWeekly);
    }
}