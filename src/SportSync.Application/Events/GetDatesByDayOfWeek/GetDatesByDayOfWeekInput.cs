using SportSync.Application.Core.Abstractions.Common;

namespace SportSync.Application.Events.GetDatesByDayOfWeek;

public class GetDatesByDayOfWeekInput : IInput<GetDatesByDayOfWeekResponse>
{
    public DayOfWeek DayOfWeek { get; set; }
    public int NumberOfDates { get; set; }
}