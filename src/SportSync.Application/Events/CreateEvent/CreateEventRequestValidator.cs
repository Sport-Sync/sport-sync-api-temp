﻿using FluentValidation;

namespace SportSync.Application.Events.CreateEvent;

public class CreateEventRequestValidator : AbstractValidator<CreateEventInput>
{
    public CreateEventRequestValidator()
    {
        RuleForEach(x => x.EventTime)
            .Where(x => x.StartDate.Date == DateTime.Today)
            .Must(x => x.StartTime.TimeOfDay > DateTime.UtcNow.TimeOfDay)
            .WithMessage("The time for today is invalid.");

        RuleForEach(x => x.EventTime)
            .Must(x => x.StartDate.Date >= DateTime.Today)
            .WithMessage("StartDate should not be in past.")
            .Must(x => x.StartTime <= x.EndTime)
            .WithMessage("StartTime needs to be earlier than EndTime.")
            .Must(x => x.StartDate.Date == x.StartTime.DateTime.Date)
            .WithMessage("StartTime needs to have the same date as StartDate.")
            .Must(x => x.StartDate.Date == x.EndTime.DateTime.Date)
            .WithMessage("EndTime needs to have the same date as StartDate.")
            .Must(x => x.StartDate.DayOfWeek == x.DayOfWeek)
            .WithMessage("StartDate should be on the same day as 'DayOfWeek' input.");

        RuleFor(x => x.NumberOfPlayers)
            .Must(number => number > 0 && number < 100)
            .WithMessage("Number of players needs to be between 0 and 100");
    }
}