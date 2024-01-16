using SportSync.Application.Core.Abstractions.Common;

namespace SportSync.Application.Events.GetDatesByDayOfWeek;

public class GetDatesByDayOfWeekInputHandler : IInputHandler<GetDatesByDayOfWeekInput, GetDatesByDayOfWeekResponse>
{
    public Task<GetDatesByDayOfWeekResponse> Handle(GetDatesByDayOfWeekInput input, CancellationToken cancellationToken)
    {
        // TODO: make validation for numberOfDates to be in range
        var futureDates = new List<DateTime>();
        var currentDate = DateTime.Today;

        var daysUntilNextDay = ((int)input.DayOfWeek - (int)currentDate.DayOfWeek + 7) % 7;
        var nextDate = currentDate.AddDays(daysUntilNextDay);

        for (int i = 0; i < input.NumberOfDates; i++)
        {
            futureDates.Add(nextDate.Date);
            nextDate = nextDate.AddDays(7);
        }

        return Task.FromResult(new GetDatesByDayOfWeekResponse
        {
            Dates = futureDates
        });
    }
}