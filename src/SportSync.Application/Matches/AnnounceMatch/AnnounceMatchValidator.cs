using FluentValidation;

namespace SportSync.Application.Matches.AnnounceMatch;

public class AnnounceMatchValidator : AbstractValidator<AnnounceMatchInput>
{
    public AnnounceMatchValidator()
    {
        RuleFor(x => x.PlayerLimit)
            .GreaterThan(0)
            .WithMessage("Number of players needs to be at least 1")
            .LessThan(20)
            .WithMessage("Number of players needs to be less than 20");
    }
}