﻿using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Types;

namespace SportSync.Api.Tests.Features.Users;

[Collection("IntegrationTests")]
public class GetUserProfileTests : IntegrationTest
{
    [Fact]
    public async Task GetUserProfile_ShouldReturn_NotFound()
    {
        var currentUser = Database.AddUser();

        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{Guid.NewGuid()}""}}){{
                        id, firstName, lastName, email, phone, imageUrl, hasPendingFriendshipRequest,
                        pendingFriendshipRequest{{
                            friendshipRequestId, 
                            sentByMe
                        }},
                        mutualFriends{{
                            id, firstName, lastName, imageUrl
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.User.NotFound);
    }

    [Fact]
    public async Task GetUserProfile_ShouldReturn_PendingFriendshipRequestSentByCurrentUser()
    {
        var currentUser = Database.AddUser();
        var requestProfile = Database.AddUser("Michael", "Scott");

        await Database.SaveChangesAsync();

        var friendshipRequest = Database.AddFriendshipRequest(currentUser, requestProfile);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{requestProfile.Id}""}}){{
                        id, firstName, lastName, email, phone, imageUrl, hasPendingFriendshipRequest,
                        pendingFriendshipRequest{{
                            friendshipRequestId, 
                            sentByMe
                        }},
                        mutualFriends{{
                            id, firstName, lastName, imageUrl
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileType>("userProfile");

        profileResponse.HasPendingFriendshipRequest.Should().BeTrue();
        profileResponse.PendingFriendshipRequest.FriendshipRequestId.Should().Be(friendshipRequest.Id);
        profileResponse.PendingFriendshipRequest.SentByMe.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserProfile_ShouldReturn_PendingFriendshipRequestSentByOtherUser()
    {
        var currentUser = Database.AddUser();
        var requestProfile = Database.AddUser("Michael", "Scott");

        await Database.SaveChangesAsync();

        var friendshipRequest = Database.AddFriendshipRequest(requestProfile, currentUser);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{requestProfile.Id}""}}){{
                        id, firstName, lastName, email, phone, imageUrl, hasPendingFriendshipRequest,
                        pendingFriendshipRequest{{
                            friendshipRequestId, 
                            sentByMe
                        }},
                        mutualFriends{{
                            id, firstName, lastName, imageUrl
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileType>("userProfile");

        profileResponse.HasPendingFriendshipRequest.Should().BeTrue();
        profileResponse.PendingFriendshipRequest.FriendshipRequestId.Should().Be(friendshipRequest.Id);
        profileResponse.PendingFriendshipRequest.SentByMe.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserProfile_ShouldNotReturnPendingFriendshipRequest_WhenAccepted()
    {
        var currentUser = Database.AddUser();
        var requestProfile = Database.AddUser("Michael", "Scott");

        await Database.SaveChangesAsync();

        var friendshipRequest = Database.AddFriendshipRequest(requestProfile, currentUser, accepted: true);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{requestProfile.Id}""}}){{
                        id, firstName, lastName, email, phone, imageUrl, hasPendingFriendshipRequest,
                        pendingFriendshipRequest{{
                            friendshipRequestId, 
                            sentByMe
                        }},
                        mutualFriends{{
                            id, firstName, lastName, imageUrl
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileType>("userProfile");

        profileResponse.HasPendingFriendshipRequest.Should().BeFalse();
        profileResponse.PendingFriendshipRequest.Should().BeNull();
    }

    [Fact]
    public async Task GetUserProfile_ShouldNotReturnPendingFriendshipRequest_WhenRejected()
    {
        var currentUser = Database.AddUser();
        var requestProfile = Database.AddUser("Michael", "Scott");

        await Database.SaveChangesAsync();

        var friendshipRequest = Database.AddFriendshipRequest(requestProfile, currentUser, rejected: true);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{requestProfile.Id}""}}){{
                        id, firstName, lastName, email, phone, imageUrl, hasPendingFriendshipRequest,
                        pendingFriendshipRequest{{
                            friendshipRequestId, 
                            sentByMe
                        }},
                        mutualFriends{{
                            id, firstName, lastName, imageUrl
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileType>("userProfile");

        profileResponse.HasPendingFriendshipRequest.Should().BeFalse();
        profileResponse.PendingFriendshipRequest.Should().BeNull();
    }

    [Fact]
    public async Task GetUserProfile_ShouldReturn_MutualFriends()
    {
        var currentUser = Database.AddUser();
        var requestProfile = Database.AddUser("Michael", "Scott");
        var mutualFriend1 = Database.AddUser();
        var mutualFriend2 = Database.AddUser();
        var onlyCurrentUserFriend = Database.AddUser();
        var onlyOtherUserFriend = Database.AddUser();

        Database.AddFriendship(currentUser, mutualFriend1);
        Database.AddFriendship(mutualFriend2, currentUser);
        Database.AddFriendship(currentUser, onlyCurrentUserFriend);
        Database.AddFriendship(requestProfile, onlyOtherUserFriend);
        Database.AddFriendship(requestProfile, mutualFriend2);
        Database.AddFriendship(mutualFriend1, requestProfile);

        await Database.SaveChangesAsync();

        var friendshipRequest = Database.AddFriendshipRequest(currentUser, requestProfile);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{requestProfile.Id}""}}){{
                        id, firstName, lastName, email, phone, imageUrl, hasPendingFriendshipRequest,
                        pendingFriendshipRequest{{
                            friendshipRequestId, 
                            sentByMe
                        }},
                        mutualFriends{{
                            id, firstName, lastName, imageUrl
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileType>("userProfile");

        profileResponse.MutualFriends.Count.Should().Be(2);
        profileResponse.MutualFriends.FirstOrDefault(x => x.Id == mutualFriend1.Id).Should().NotBeNull();
        profileResponse.MutualFriends.FirstOrDefault(x => x.Id == mutualFriend2.Id).Should().NotBeNull();
    }
}