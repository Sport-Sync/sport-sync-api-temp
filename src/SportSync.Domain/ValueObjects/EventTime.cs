using System.Text;
using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.ValueObjects;

public sealed class EventTime : ValueObject
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool RepeatWeekly { get; set; }

    private EventTime(DateOnly date, TimeOnly startTime, TimeOnly endTime, bool repeatWeekly)
    {
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        RepeatWeekly = repeatWeekly;
    }

    public string Value => ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        throw new NotImplementedException();
    }

    public static EventTime Create(DateOnly date, TimeOnly startTime, TimeOnly endTime, bool repeatWeekly)
    {
        return new EventTime(date, startTime, endTime, repeatWeekly);
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        if (RepeatWeekly)
        {
            sb.Append(Date.DayOfWeek);
        }
        else
        {
            sb.Append(Date.ToShortDateString());
        }

        sb.Append("(");
        sb.Append(StartTime.ToShortTimeString());
        sb.Append("-");
        sb.Append(EndTime.ToShortTimeString());
        sb.Append(")");

        return sb.ToString();
    }
}