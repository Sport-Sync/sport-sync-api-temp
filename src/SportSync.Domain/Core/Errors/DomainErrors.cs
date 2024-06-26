﻿using SportSync.Domain.Core.Primitives;
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

        public static Error NotFriends => new("User.NotFriends", "The users are not friends.");

        public static Error CannotChangePassword => new (
            "User.CannotChangePassword",
            "The password cannot be changed to the specified password.");
    }

    public static class Match
    {
        public static Error NotFound => new("Match.NotFound", "The match with the specified identifier was not found.");
        public static Error PlayerNotFound => new("Match.PlayerNotFound", "Identified player is not part of this match.");
        public static Error AlreadyFinished => new("Match.AlreadyFinished", "The match has already finished.");
        public static Error InProgress => new("Match.InProgress", "The match is in progress.");
        public static Error Canceled => new("Match.Canceled", "The match has been canceled.");
        public static Error AlreadyPlayer => new ("Match.AlreadyPlayer", "The user is already a player in this match.");
    }

    public static class Event
    {
        public static Error NotMember => new("Event.NotMember", "The user is not a member of an event.");
        public static Error NotFound => new("Event.NotFound", "The event with the specified identifier was not found.");
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

    public static class EventInvitation
    {
        public static Error NotFound => new Error(
            "EventInvitation.NotFound",
            "The event invitation with the specified identifier was not found.");

        public static Error AlreadyAccepted => new Error(
            "EventInvitation.AlreadyAccepted",
            "The invitation has already been accepted.");

        public static Error AlreadyRejected => new Error(
            "EventInvitation.AlreadyRejected",
            "The invitation has already been rejected.");

        public static Error AlreadyMember => new("EventInvitation.AlreadyMember", "The user is already a member on this event.");

        public static Error AlreadyInvited => new("EventInvitation.AlreadyInvited", "The user is already invited to this event. Waiting for response.");
    }

    public static class MatchApplication
    {
        public static Error NotFound => new Error(
            "MatchApplication.NotFound",
            "The match application with the specified identifier was not found.");

        public static Error PlayersLimitReached => new Error(
            "MatchApplication.PlayersLimitReached",
            "The limit of players needed for this match has already been reached.");

        public static Error AlreadyAccepted => new Error(
            "MatchApplication.AlreadyAccepted",
            "The application has already been accepted.");

        public static Error AlreadyRejected => new Error(
            "MatchApplication.AlreadyRejected",
            "The application has already been rejected.");

        public static Error AlreadyCanceled => new Error(
            "MatchApplication.AlreadyCanceled",
            "The application has already been canceled by user.");

        public static Error NotAnnounced => new Error(
            "MatchApplication.NotAnnounced",
            "The match is not announced. Unable to perform the operation.");

        public static Error NotOnFriendList => new Error(
            "MatchApplication.NotOnFriendList",
            "The match is announced only for friends. The user is not on the friend list.");


        public static Error PendingMatchApplication => new Error(
            "MatchApplication.PendingMatchApplication",
            "The application can not be sent because there is a pending one already.");
    }

    public static class MatchAnnouncement
    {
        public static Error UserIsNotPlayer => new Error(
            "MatchAnnouncement.UserIsNotPlayer",
            "Current user is not a player on this match.");

        public static Error AlreadyPubliclyAnnounced => new Error(
            "MatchAnnouncement.AlreadyPubliclyAnnounced",
            "The match is already publicly announced.");

        public static Error AlreadyAnnounced => new Error(
            "MatchAnnouncement.AlreadyAnnounced",
            "The match is already announced.");

        public static Error NotAnnounced => new Error(
            "MatchAnnouncement.NotAnnounced",
            "The match is not announced.");

        public static Error PlayerLimitLessThanAlreadyAccepted => new Error(
            "MatchAnnouncement.PlayerLimitLessThanAlreadyAccepted",
            "Player limit needs to be greater than the number of players already accepted.");
    }

    public static class Email
    {
        public static Error NullOrEmpty => new Error("Email.NullOrEmpty", "The email is required.");

        public static Error LongerThanAllowed => new Error("Email.LongerThanAllowed", "The email is longer than allowed.");

        public static Error InvalidFormat => new Error("Email.InvalidFormat", "The email format is invalid.");
    }

    public static class PhoneNumber
    {
        public static Error NullOrEmpty => new Error("PhoneNumber.NullOrEmpty", "The phone number is required.");

        public static Error InvalidFormat => new Error("Email.InvalidFormat", "The email format is invalid.");
    }

    public static class Notification
    {
        public static Error TooManyActions => new Error("Notification.TooManyActions", "Notification can not have more than 3 actions.");
        
        public static Error NotFound => new Error("Notification.NotFound", "Notification with specified id was not found.");

        public static Error ContentNotImplemented => new Error("Notification.ContentNotImplemented", "Content not implemented for specified notification type.");

        public static Error CommandNotFound => new Error("Notification.CommandNotFound", "Command was not found for this notification.");
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