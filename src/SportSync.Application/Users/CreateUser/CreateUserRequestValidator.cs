using FluentValidation;
using SportSync.Domain.Core.Utility;

namespace SportSync.Application.Users.CreateUser;

public class CreateUserRequestValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MinimumLength(3)
            .WithMessage("First name minimum length is 3.")
            .MaximumLength(10)
            .WithMessage("First name maximum length is 100.")
            .NotEmpty()
            .WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .MinimumLength(3)
            .WithMessage("Last name minimum length is 3.")
            .MaximumLength(10)
            .WithMessage("Last name maximum length is 100.")
            .NotEmpty()
            .WithMessage("Last name is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Matches(RegularExpressions.Email)
            .WithMessage("The email format is invalid.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(RegularExpressions.CroatianPhoneNumber)
            .WithMessage("Phone number is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("The password is required.");
    }
}