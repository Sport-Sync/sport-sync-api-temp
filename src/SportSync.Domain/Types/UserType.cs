﻿using System.Linq.Expressions;
using HotChocolate;
using SportSync.Domain.Entities;
using SportSync.Domain.Types.Abstraction;

namespace SportSync.Domain.Types;

public class UserType
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public UserType(User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
    }

    public UserType()
    {

    }

    public static Expression<Func<User, UserType>> PropertySelector = x => new UserType
    {
        Id = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone.Value
    };
}

public class UserProfileType : UserType, IPendingFriendshipRequestsInfo
{
    public string ImageUrl { get; set; }
    public List<FriendType> MutualFriends { get; set; }
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }
    public bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;

    public UserProfileType(User user, PendingFriendshipRequestType pendingFriendshipRequest = null, List<FriendType> mutualFriends = null, string imageUrl = null)
        : base(user)
    {

        PendingFriendshipRequest = pendingFriendshipRequest;
        MutualFriends = mutualFriends;
        ImageUrl = imageUrl;
    }

    public UserProfileType(User user, string imageUrl = null)
        : base(user)
    {
        ImageUrl = imageUrl;
    }

    public UserProfileType()
    {
        
    }
}

public class FriendType : UserType
{
    public string ImageUrl { get; set; }
    [GraphQLIgnore]
    public bool HasProfileImage { get; set; }

    public FriendType(User user, string imageUrl = null)
        : base(user)
    {
        ImageUrl = imageUrl;
    }

    public FriendType()
    {
        
    }

    public static Expression<Func<User, FriendType>> PropertySelector = x => new FriendType
    {
        Id = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone.Value,
        HasProfileImage = x.HasProfileImage
    };
}

public class PhoneBookUserType : UserType, IPendingFriendshipRequestsInfo
{
    public PendingFriendshipRequestType PendingFriendshipRequest { get; set; }
    public bool HasPendingFriendshipRequest => PendingFriendshipRequest != null;

    public PhoneBookUserType(UserType user, PendingFriendshipRequestType pendingFriendshipRequest = null)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        Phone = user.Phone;
        PendingFriendshipRequest = pendingFriendshipRequest;
    }

    public PhoneBookUserType()
    {
        
    }
}