﻿using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Services;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Entities;

public class User : AggregateRoot
{
    private string _passwordHash;
    private readonly HashSet<Friendship> _friendInviters = new();
    private readonly HashSet<Friendship> _friendInvitees = new();

    private User(string firstName, string lastName, string email, PhoneNumber phone, string passwordHash)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(firstName, "The first name is required.", nameof(firstName));
        Ensure.NotEmpty(lastName, "The last name is required.", nameof(lastName));
        Ensure.NotEmpty(email, "The email is required.", nameof(email));
        Ensure.NotNull(phone, "The phone is required.", nameof(phone));
        Ensure.NotEmpty(passwordHash, "The password hash is required", nameof(passwordHash));

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        _passwordHash = passwordHash;
    }

    private User()
    {
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; }

    public PhoneNumber Phone { get; set; }

    public string ImageUrl { get; set; }

    /// <summary>
    /// List of users who initiated the "friendship"
    /// </summary>
    public IReadOnlyCollection<Friendship> FriendInvitees => _friendInvitees.ToList();

    /// <summary>
    /// List of users who where invited to the "friendship"
    /// </summary>
    public IReadOnlyCollection<Friendship> FriendInviters => _friendInviters.ToList();
    public IEnumerable<Guid> Friends => _friendInvitees.Select(x => x.UserId).Concat(_friendInviters.Select(x => x.FriendId));

    public static User Create(string firstName, string lastName, string email, PhoneNumber phone, string passwordHash)
    {
        return new User(firstName, lastName, email, phone, passwordHash);
    }

    public Result ChangePassword(string passwordHash)
    {
        if (passwordHash == _passwordHash)
        {
            return Result.Failure(DomainErrors.User.CannotChangePassword);
        }

        _passwordHash = passwordHash;

        return Result.Success();
    }

    public bool VerifyPasswordHash(string password, IPasswordHashChecker passwordHashChecker)
        => !string.IsNullOrWhiteSpace(password) && passwordHashChecker.HashesMatch(_passwordHash, password);

    public Event CreateEvent(string name, SportTypeEnum sportType, string address, decimal price, int numberOfPlayers, string notes, Guid[] memberIds)
    {
        var @event = Event.Create(this, name, sportType, address, price, numberOfPlayers, notes, memberIds);

        return @event;
    }

    public Result<FriendshipRequest> SendFriendshipRequest(User friend)
    {
        if (Id == friend.Id)
        {
            return Result.Failure<FriendshipRequest>(DomainErrors.FriendshipRequest.InvalidSameUserId);
        }

        if (IsFriendWith(friend))
        {
            return Result.Failure<FriendshipRequest>(DomainErrors.FriendshipRequest.AlreadyFriends);
        }

        var friendshipRequest = new FriendshipRequest(this, friend);

        RaiseDomainEvent(new FriendshipRequestSentDomainEvent(friendshipRequest));

        return friendshipRequest;
    }

    public Result<Friendship> AddFriend(User friend)
    {
        if (IsFriendWith(friend))
        {
            return Result.Failure<Friendship>(DomainErrors.FriendshipRequest.AlreadyFriends);
        }

        var friendship = new Friendship(this, friend);

        _friendInviters.Add(friendship);

        return friendship;
    }

    public Result RemoveFriend(Guid friendId)
    {
        var removed = _friendInvitees.RemoveWhere(x => x.UserId == friendId);

        if (removed > 0)
        {
            return Result.Success();
        }

        removed = _friendInviters.RemoveWhere(x => x.FriendId == friendId);
        return removed > 0 ?
            Result.Success() :
            Result.Failure<Friendship>(DomainErrors.FriendshipRequest.FriendNotFound);
    }

    public Result NotifyFriendsAboutMatchAnnouncement(Match match)
    {
        if (!match.Announced)
        {
            return Result.Failure(DomainErrors.MatchAnnouncement.NotAnnounced);
        }

        if (!match.IsPlayer(Id))
        {
            return Result.Failure(DomainErrors.MatchAnnouncement.UserIsNotPlayer);
        }

        var player = match.Players.First(x => x.UserId == Id);
        player.SetAsMatchAnnouncer();

        RaiseDomainEvent(new MatchAnnouncedToFriendsDomainEvent(match, this));

        return Result.Success();
    }

    public bool IsFriendWith(User friend)
    {
        return Friends.Any(id => id == friend.Id);
    }

    public bool IsFriendWith(Guid friendId)
    {
        return Friends.Any(id => id == friendId);
    }

    public Result AddFriendToMatch(Guid friendId, Match match)
    {
        if (!match.IsPlayer(Id))
        {
            return Result.Failure(DomainErrors.Match.PlayerNotFound);
        }

        if (match.IsPlayer(friendId))
        {
            return Result.Failure(DomainErrors.Match.AlreadyPlayer);
        }

        if (!IsFriendWith(friendId))
        {
            return Result.Failure(DomainErrors.User.NotFriends);
        }

        var matchPendingResult = match.ValidateItIsPendingStatus();

        if (matchPendingResult.IsFailure)
        {
            return matchPendingResult;
        }

        match.AddPlayer(friendId);

        RaiseDomainEvent(new FriendAddedToMatchDomainEvent(this, friendId, match));

        return Result.Success();
    }
}