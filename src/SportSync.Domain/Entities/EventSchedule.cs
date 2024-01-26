using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class EventSchedule : Entity
{
    private EventSchedule(DayOfWeek dayOfWeek, DateTime startDate, DateTime startTimeUtc, DateTime endTimeUtc, bool repeatWeekly)
        : base(Guid.NewGuid())
    {
        DayOfWeek = dayOfWeek;
        StartDate = startDate;
        StartTimeUtc = startTimeUtc;
        EndTimeUtc = endTimeUtc;
        RepeatWeekly = repeatWeekly;
    }

    private EventSchedule()
    {
    }

    public Guid EventId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public bool RepeatWeekly { get; set; }
    public Event Event { get; set; }

    public static EventSchedule Create(DayOfWeek dayOfWeek, DateTime startDate, DateTime startTime, DateTime endTime, bool repeatWeekly)
    {
        return new EventSchedule(dayOfWeek, startDate, startTime, endTime, repeatWeekly);
    }
}