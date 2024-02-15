using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Enumerations;

namespace SportSync.Domain.Core.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static Error NotFound => new ("User.NotFound", "The user with the specified identifier was not found.");

        public static Error IdUnavailable => new("User.IdUnavailable", "User id is unavailable.");

        public static Error DuplicateEmail => new ("User.DuplicateEmail", "The specified email is already in use.");

        public static Error DuplicatePhone => new("User.DuplicatePhone", "The specified phone number is already in use.");

        public static Error Forbidden => new("User.Forbidden", "The current user does not have the permissions to perform this operation.");

        public static Error CannotChangePassword => new (
            "User.CannotChangePassword",
            "The password cannot be changed to the specified password.");
    }

    public static class Termin
    {
        public static Error NotFound => new("Termin.NotFound", "The termin with the specified identifier was not found.");
        public static Error PlayerNotFound => new("Termin.PlayerNotFound", "Identified player is not part of this termin.");
        public static Error AlreadyFinished => new("Termin.AlreadyFinished", "The termin has already finished or is in progress.");
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

    public static class FriendshipRequest
    {
        public static Error NotFound => new Error(
            "FriendshipRequest.NotFound",
            "The friendship request with the specified identifier was not found.");

        public static Error FriendNotFound => new Error(
            "FriendshipRequest.FriendNotFound",
            "The friend with the specified identifier was not found.");

        public static Error AlreadyAccepted => new Error(
            "FriendshipRequest.AlreadyAccepted",
            "The friendship request has already been accepted.");

        public static Error AlreadyRejected => new Error(
            "FriendshipRequest.AlreadyRejected",
            "The friendship request has already been rejected.");

        public static Error InvalidSameUserId => new Error(
            "FriendshipRequest.InvalidRequest",
            "The user cannot make friendship with himself.");

        public static Error AlreadyFriends => new Error(
            "FriendshipRequest.AlreadyFriends",
            "The friendship request can not be sent because the users are already friends.");

        public static Error PendingFriendshipRequest => new Error(
            "FriendshipRequest.PendingFriendshipRequest",
            "The friendship request can not be sent because there is a pending friendship request.");
    }

    public static class TerminApplication
    {
        public static Error NotFound => new Error(
            "TerminApplication.NotFound",
            "The termin application with the specified identifier was not found.");

        public static Error AlreadyAccepted => new Error(
            "TerminApplication.AlreadyAccepted",
            "The application has already been accepted.");

        public static Error AlreadyRejected => new Error(
            "TerminApplication.AlreadyRejected",
            "The application has already been rejected.");

        public static Error AlreadyPlayer => new Error(
            "TerminApplication.AlreadyPlayer",
            "The user is already a player in this termin.");

        public static Error NotAnnounced => new Error(
            "TerminApplication.NotAnnounced",
            "The termin is not announced. Unable to perform the operation.");

        public static Error NotOnFriendList => new Error(
            "TerminApplication.NotOnFriendList",
            "The termin is announced only for friends. The user is not on the friend list.");


        public static Error PendingTerminApplication => new Error(
            "TerminApplication.PendingTerminApplication",
            "The application can not be sent because there is a pending one already.");
    }

    public static class TerminAnnouncement
    {
        public static Error AlreadyPubliclyAnnounced => new Error(
            "TerminAnnouncement.AlreadyPubliclyAnnounced",
            "The termin is already publicly announced.");

        public static Error AlreadyAnnouncedBySameUser => new Error(
            "TerminAnnouncement.AlreadyAnnouncedBySameUser",
            "The termin is already announced by same user.");
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