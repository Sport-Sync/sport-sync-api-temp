using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Core.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static Error NotFound => new ("User.NotFound", "The user with the specified identifier was not found.");

        public static Error DuplicateEmail => new ("User.DuplicateEmail", "The specified email is already in use.");

        public static Error DuplicatePhone => new("User.DuplicatePhone", "The specified phone number is already in use.");

        public static Error CannotChangePassword => new (
            "User.CannotChangePassword",
            "The password cannot be changed to the specified password.");
    }

    public static class Password
    {
        public static Error NullOrEmpty => new ("Password.NullOrEmpty", "The password is required.");

        public static Error TooShort => new ("Password.TooShort", "The password is too short.");

        public static Error MissingUppercaseLetter => new (
            "Password.MissingUppercaseLetter",
            "The password requires at least one uppercase letter.");

        public static Error MissingLowercaseLetter => new (
            "Password.MissingLowercaseLetter",
            "The password requires at least one lowercase letter.");

        public static Error MissingDigit => new (
            "Password.MissingDigit",
            "The password requires at least one digit.");
    }

    public static class Email
    {
        public static Error NullOrEmpty => new Error("Email.NullOrEmpty", "The email is required.");

        public static Error LongerThanAllowed => new Error("Email.LongerThanAllowed", "The email is longer than allowed.");

        public static Error InvalidFormat => new Error("Email.InvalidFormat", "The email format is invalid.");
    }

    public static class General
    {
        public static Error UnProcessableRequest => new (
            "General.UnProcessableRequest",
            "The server could not process the request.");

        public static Error ServerError => new ("General.ServerError", "The server encountered an unrecoverable error.");
    }

    public static class Authentication
    {
        public static Error InvalidEmailOrPassword => new (
            "Authentication.InvalidEmailOrPassword",
            "The specified email or password are incorrect.");
    }
}