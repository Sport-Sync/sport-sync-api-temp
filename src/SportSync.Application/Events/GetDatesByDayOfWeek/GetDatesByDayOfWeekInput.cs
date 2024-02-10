using SportSync.Application.Core.Abstractions.Common;

namespace SportSync.Application.Events.GetDatesByDayOfWeek;

public class GetDatesByDayOfWeekInput : IRequest<GetDatesByDayOfWeekResponse>
{
    public DayOfWeek DayOfWeek { get; set; }
    public int NumberOfDates { get; set; }
}