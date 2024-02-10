using FluentValidation;
using SportSync.Domain.Core.Utility;

namespace SportSync.Application.Authentication.Login;

public class LoginRequestValidator : AbstractValidator<LoginInput>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Matches(RegularExpressions.Email)
            .WithMessage("The email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}