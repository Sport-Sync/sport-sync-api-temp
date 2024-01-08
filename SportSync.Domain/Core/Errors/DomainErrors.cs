using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Core.Errors;

public static class DomainErrors
{

    public static class User
    {
        public static Error NotFound => new ("User.NotFound", "The user with the specified identifier was not found.");

        public static Error DuplicateEmail => new ("User.DuplicateEmail", "The specified email is already in use.");

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

        public static Error MissingNonAlphaNumeric => new (
            "Password.MissingNonAlphaNumeric",
            "The password requires at least one non-alphanumeric.");
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