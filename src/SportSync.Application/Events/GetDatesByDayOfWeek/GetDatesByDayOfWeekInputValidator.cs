using FluentValidation;

namespace SportSync.Application.Events.GetDatesByDayOfWeek;

public class GetDatesByDayOfWeekInputValidator : AbstractValidator<GetDatesByDayOfWeekInput>
{
    public GetDatesByDayOfWeekInputValidator()
    {
        RuleFor(x => x.NumberOfDates)
            .Must(x => x > 0 && x <= 50)
            .WithMessage("NumberOfDates needs to be greater than 0 and less than 51");
    }
}