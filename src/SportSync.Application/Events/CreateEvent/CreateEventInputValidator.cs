using FluentValidation;

namespace SportSync.Application.Events.CreateEvent;

public class CreateEventInputValidator : AbstractValidator<CreateEventInput>
{
    public CreateEventInputValidator()
    {
        RuleForEach(x => x.EventTime)
            .Must(x => x.StartDate.DayOfWeek == x.DayOfWeek)
            .WithMessage("StartDate should be on the same day as 'DayOfWeek' input");
    }
}