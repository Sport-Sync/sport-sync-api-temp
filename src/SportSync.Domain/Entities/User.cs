﻿using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Core.Utility;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;
using SportSync.Domain.Services;

namespace SportSync.Domain.Entities;

public class User : AggregateRoot
{
    private string _passwordHash;

    private User(string firstName, string lastName, string email, string phone, string passwordHash)
        : base(Guid.NewGuid())
    {
        Ensure.NotEmpty(firstName, "The first name is required.", nameof(firstName));
        Ensure.NotEmpty(lastName, "The last name is required.", nameof(lastName));
        Ensure.NotEmpty(email, "The email is required.", nameof(email));
        Ensure.NotEmpty(phone, "The phone is required.", nameof(phone));
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

    public string Phone { get; set; }

    public static User Create(string firstName, string lastName, string email, string phone, string passwordHash)
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

    public Event CreateEvent(string name, SportType sportType, string address, decimal price, int numberOfPlayers, string notes)
    {
        var @event = Event.Create(this, name, sportType, address, price, numberOfPlayers, notes);

        return @event;
    }

    public async Task<Result<FriendshipRequest>> SendFriendshipRequest(IUserRepository userRepository, IFriendshipRequestRepository friendshipRequestRepository, User friend)
    {
        if (Id == friend.Id)
        {
            return Result.Failure<FriendshipRequest>(DomainErrors.FriendshipRequest.InvalidSameUserId);
        }

        if (await userRepository.CheckIfFriendsAsync(this, friend))
        {
            return Result.Failure<FriendshipRequest>(DomainErrors.FriendshipRequest.AlreadyFriends);
        }

        if (await friendshipRequestRepository.CheckForPendingFriendshipRequestAsync(this, friend))
        {
            return Result.Failure<FriendshipRequest>(DomainErrors.FriendshipRequest.PendingFriendshipRequest);
        }

        var friendshipRequest = new FriendshipRequest(this, friend);

        //RaiseDomainEvent(new FriendshipRequestSentDomainEvent(friendshipRequest));

        return friendshipRequest;
    }
}