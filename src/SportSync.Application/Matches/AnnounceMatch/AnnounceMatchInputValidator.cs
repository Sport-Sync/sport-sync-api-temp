using FluentValidation;

namespace SportSync.Application.Matches.AnnounceMatch;

public class AnnounceMatchInputValidator : AbstractValidator<AnnounceMatchInput>
{
    public AnnounceMatchInputValidator()
    {
        RuleFor(x => x.PlayerLimit)
            .GreaterThan(0)
            .WithMessage("PlayerLimit needs to be greater than 0");

        RuleFor(x => x.PlayerLimit)
            .LessThan(100)
            .WithMessage("PlayerLimit needs to be less than 100");
    }
}