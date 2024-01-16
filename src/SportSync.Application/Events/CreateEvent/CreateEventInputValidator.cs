using FluentValidation;

namespace SportSync.Application.Events.CreateEvent;

public class CreateEventInputValidator : AbstractValidator<CreateEventInput>
{
    public CreateEventInputValidator()
    {
        RuleForEach(x => x.EventTime)
            .Must(x => x.StartDate.Date > DateTime.Today)
            .WithMessage("StartDate should be after today.")
            .Must(x => x.StartDate.DayOfWeek == x.DayOfWeek)
            .WithMessage("StartDate should be on the same day as 'DayOfWeek' input");

        RuleFor(x => x.NumberOfPlayers)
            .Must(number => number > 0 && number < 100)
            .WithMessage("Number of players needs to be between 0 and 100");
    }
}