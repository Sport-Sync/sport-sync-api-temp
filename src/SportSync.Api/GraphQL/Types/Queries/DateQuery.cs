namespace sport_sync.GraphQL.Types.Queries;

[ExtendObjectType("Query")]
public class DateQuery
{
    public DatesResponse GetDatesByDayOfWeek(GetDatesInput input)
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

        return new DatesResponse()
        {
            Dates = futureDates
        };
    }
}

// TODO: move to files
public class DatesResponse
{
    public List<DateTime> Dates { get; set; }
}

public class GetDatesInput
{
    public DayOfWeek DayOfWeek { get; set; }
    public int NumberOfDates { get; set; }
}