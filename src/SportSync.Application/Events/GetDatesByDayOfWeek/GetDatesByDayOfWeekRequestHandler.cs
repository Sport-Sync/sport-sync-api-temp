﻿using SportSync.Application.Core.Abstractions.Common;

namespace SportSync.Application.Events.GetDatesByDayOfWeek;

public class GetDatesByDayOfWeekRequestHandler : IRequestHandler<GetDatesByDayOfWeekInput, GetDatesByDayOfWeekResponse>
{
    private readonly IDateTime _dateTime;

    public GetDatesByDayOfWeekRequestHandler(IDateTime dateTime)
    {
        _dateTime = dateTime;
    }

    public Task<GetDatesByDayOfWeekResponse> Handle(GetDatesByDayOfWeekInput input, CancellationToken cancellationToken)
    {
        var futureDates = new List<DateTime>();
        var currentDate = _dateTime.UtcNow.Date;

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