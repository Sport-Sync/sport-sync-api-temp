using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Users.GetUserProfile;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Users;

[Collection("IntegrationTests")]
public class GetUserProfileTests : IntegrationTest
{
    public GetUserProfileTests()
    {
        DateTimeProviderMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
    }

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
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.User.NotFound);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetUserProfile_ShouldReturn_IsUserFriendWithCurrentUser(bool areFriends)
    {
        var currentUser = Database.AddUser();
        var requestProfile = Database.AddUser("Michael", "Scott");
        await Database.SaveChangesAsync();

        if (areFriends)
        {
            currentUser.AddFriend(requestProfile);
            await Database.SaveChangesAsync();
        }

        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{requestProfile.Id}""}}){{
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileResponse>("userProfile");

        profileResponse.User.IsFriendWithCurrentUser.Should().Be(areFriends);
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
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileResponse>("userProfile");

        profileResponse.User.PendingFriendshipRequestId.Should().Be(friendshipRequest.Id);
        profileResponse.User.IsFriendshipRequestSentByCurrentUser.Should().BeTrue();
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
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileResponse>("userProfile");

        profileResponse.User.PendingFriendshipRequestId.Should().Be(friendshipRequest.Id);
        profileResponse.User.IsFriendshipRequestSentByCurrentUser.Should().BeFalse();
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
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileResponse>("userProfile");

        profileResponse.User.PendingFriendshipRequestId.Should().BeNull();
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
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileResponse>("userProfile");

        profileResponse.User.PendingFriendshipRequestId.Should().BeNull();
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
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileResponse>("userProfile");

        profileResponse.User.MutualFriends.Count.Should().Be(2);
        profileResponse.User.MutualFriends.FirstOrDefault(x => x.Id == mutualFriend1.Id).Should().NotBeNull();
        profileResponse.User.MutualFriends.FirstOrDefault(x => x.Id == mutualFriend2.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserProfile_ShouldReturn_MatchApplications()
    {
        var currentUser = Database.AddUser();
        var otherUser = Database.AddUser();
        var requestProfile = Database.AddUser("Michael", "Scott");

        var match1 = Database.AddMatch(currentUser, startDate: DateTime.Today.AddDays(1));
        var match2 = Database.AddMatch(currentUser, startDate: DateTime.Today.AddDays(1));
        var acceptedMatch = Database.AddMatch(currentUser, startDate: DateTime.Today.AddDays(1));
        var notAnnouncedMatch = Database.AddMatch(currentUser, startDate: DateTime.Today.AddDays(1));
        var matchByOtherUser = Database.AddMatch(otherUser, startDate: DateTime.Today.AddDays(1));

        await Database.SaveChangesAsync();

        match1.Announce(currentUser, true, 1);
        match2.Announce(currentUser, true, 1);
        acceptedMatch.Announce(currentUser, true, 1);
        matchByOtherUser.Announce(otherUser, true, 1);
        
        var application1Result = match1.ApplyForPlaying(requestProfile);
        var application2Result = match2.ApplyForPlaying(requestProfile);
        var application3Result = acceptedMatch.ApplyForPlaying(requestProfile);
        var applicationForOtherUser = matchByOtherUser.ApplyForPlaying(requestProfile);
        
        Database.DbContext.Set<MatchApplication>().Add(application1Result.Value);
        Database.DbContext.Set<MatchApplication>().Add(application2Result.Value);
        Database.DbContext.Set<MatchApplication>().Add(applicationForOtherUser.Value);

        application3Result.Value.Accept(currentUser, acceptedMatch, DateTime.UtcNow);

        await Database.SaveChangesAsync();
        
        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{requestProfile.Id}""}}){{
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }},
                        matchApplications{{
                            matchId, matchApplicationId, matchName
                        }}
                    }}
                }}"));

        var profileResponse = result.ToResponseObject<UserProfileResponse>("userProfile");

        profileResponse.MatchApplications.Count.Should().Be(2);
        var match1Application = profileResponse.MatchApplications.FirstOrDefault(x => x.MatchId == match1.Id);
        var match2Application = profileResponse.MatchApplications.FirstOrDefault(x => x.MatchId == match2.Id);

        match1Application.MatchApplicationId.Should().Be(application1Result.Value.Id);
        match1Application.MatchName.Should().Be(match1.EventName);

        match2Application.MatchApplicationId.Should().Be(application2Result.Value.Id);
        match2Application.MatchName.Should().Be(match2.EventName);
    }

    [Fact]
    public async Task GetUserProfile_ShouldReturn_IsCurrentUserAdminFlag()
    {
        var currentUser = Database.AddUser();
        var otherAdmin = Database.AddUser();
        var requestProfile = Database.AddUser("Michael", "Scott");

        var match1 = Database.AddMatch(currentUser, startDate: DateTime.Today.AddDays(1));
        var match2 = Database.AddMatch(otherAdmin, startDate: DateTime.Today.AddDays(1));
        match2.AddPlayer(currentUser.Id);

        await Database.SaveChangesAsync();

        match1.Announce(currentUser, true, 1);
        match2.Announce(otherAdmin, true, 1);
        
        var application1Result = match1.ApplyForPlaying(requestProfile);
        var application2Result = match2.ApplyForPlaying(requestProfile);

        Database.DbContext.Set<MatchApplication>().Add(application1Result.Value);
        Database.DbContext.Set<MatchApplication>().Add(application2Result.Value);
        
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(currentUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                query{{
                    userProfile(input: {{userId: ""{requestProfile.Id}""}}){{
                        user{{
                            id, firstName, lastName, email, phone, imageUrl,
                            pendingFriendshipRequestId,
                            isFriendWithCurrentUser,
                            isFriendshipRequestSentByCurrentUser,
                            mutualFriends{{
                                id, firstName, lastName, imageUrl
                            }}
                        }},
                        matchApplications{{
                            matchId, matchApplicationId, matchName, isCurrentUserAdmin
                        }}
                    }}
                }}"));
        
        var profileResponse = result.ToResponseObject<UserProfileResponse>("userProfile");

        profileResponse.MatchApplications.Count.Should().Be(2);
        var match1Application = profileResponse.MatchApplications.FirstOrDefault(x => x.MatchId == match1.Id);
        var match2Application = profileResponse.MatchApplications.FirstOrDefault(x => x.MatchId == match2.Id);

        match1Application.MatchApplicationId.Should().Be(application1Result.Value.Id);
        match1Application.MatchName.Should().Be(match1.EventName);
        match1Application.IsCurrentUserAdmin.Should().BeTrue();

        match2Application.MatchApplicationId.Should().Be(application2Result.Value.Id);
        match2Application.MatchName.Should().Be(match2.EventName);
        match2Application.IsCurrentUserAdmin.Should().BeFalse();
    }
}