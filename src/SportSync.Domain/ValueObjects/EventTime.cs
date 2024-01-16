using System.Text;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.ValueObjects;

public sealed class EventTime : ValueObject
{
    public DayOfWeek DayOfWeek { get; set; }
    public DateOnly StartDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool RepeatWeekly { get; set; }

    private EventTime(DayOfWeek dayOfWeek, DateOnly startDate, TimeOnly startTime, TimeOnly endTime, bool repeatWeekly)
    {
        DayOfWeek = dayOfWeek;
        StartDate = startDate;
        StartTime = startTime;
        EndTime = endTime;
        RepeatWeekly = repeatWeekly;
    }

    //public string Value => ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return DayOfWeek;
        yield return StartDate.ToShortDateString();
        yield return StartTime.ToShortTimeString();
        yield return EndTime.ToShortTimeString();
    }

    public static EventTime Create(DayOfWeek dayOfWeek, DateOnly startDate, TimeOnly startTime, TimeOnly endTime, bool repeatWeekly)
    {
        return new EventTime(dayOfWeek, startDate, startTime, endTime, repeatWeekly);
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        if (RepeatWeekly)
        {
            sb.Append("Every ");
        }

        sb.Append(DayOfWeek);
        sb.Append(" ");
        sb.Append(StartTime.ToShortTimeString());
        sb.Append("-");
        sb.Append(EndTime.ToShortTimeString());

        if (RepeatWeekly)
        {
            sb.Append("(Only once - ");
            sb.Append(StartDate.ToShortDateString());
            sb.Append(")");
        }

        return sb.ToString();
    }
}