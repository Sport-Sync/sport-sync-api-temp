using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class EventSchedule : Entity
{
    private EventSchedule(DayOfWeek dayOfWeek, DateOnly startDate, TimeOnly startTimeUtc, TimeOnly endTimeUtc, bool repeatWeekly)
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
    public DateOnly StartDate { get; set; }
    public TimeOnly StartTimeUtc { get; set; }
    public TimeOnly EndTimeUtc { get; set; }
    public bool RepeatWeekly { get; set; }

    public static EventSchedule Create(DayOfWeek dayOfWeek, DateOnly startDate, TimeOnly startTime, TimeOnly endTime, bool repeatWeekly)
    {
        return new EventSchedule(dayOfWeek, startDate, startTime, endTime, repeatWeekly);
    }
}