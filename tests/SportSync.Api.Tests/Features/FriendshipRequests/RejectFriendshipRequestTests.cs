using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.FriendshipRequests;

[Collection("IntegrationTests")]
public class RejectFriendshipRequestTests : IntegrationTest
{
    [Fact]
    public async Task RejectFriendshipRequest_ShouldFail_WhenFriendshipRequestNotFound()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectFriendshipRequest(input: {{ 
                        friendshipRequestId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectFriendshipRequest", DomainErrors.FriendshipRequest.NotFound);
    }

    [Fact]
    public async Task RejectFriendshipRequest_ShouldFail_WhenCurrentUserIsNotInvitedFriend()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser("Friend", "Friendman");

        var friendshipRequest = Database.AddFriendshipRequest(user, friend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);


        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectFriendshipRequest(input: {{ 
                        friendshipRequestId: ""{friendshipRequest.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectFriendshipRequest", DomainErrors.User.Forbidden);

        var friendshipRequestDb = Database.DbContext.Set<FriendshipRequest>().First(x => x.UserId == user.Id && x.FriendId == friend.Id);
        friendshipRequestDb.Accepted.Should().BeFalse();
        friendshipRequestDb.Rejected.Should().BeFalse();
        friendshipRequestDb.CompletedOnUtc.Should().BeNull();
    }

    [Fact]
    public async Task RejectFriendshipRequest_ShouldFail_WhenIsAlreadyAccepted()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser("Friend", "Friendman");
        await Database.SaveChangesAsync();

        var friendshipRequest = Database.AddFriendshipRequest(user, friend, accepted: true);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(friend.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectFriendshipRequest(input: {{ 
                        friendshipRequestId: ""{friendshipRequest.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectFriendshipRequest", DomainErrors.FriendshipRequest.AlreadyAccepted);

        var friendshipRequestDb = Database.DbContext.Set<FriendshipRequest>().First(x => x.UserId == user.Id && x.FriendId == friend.Id);
        friendshipRequestDb.Accepted.Should().BeTrue();
        friendshipRequestDb.Rejected.Should().BeFalse();
        friendshipRequestDb.CompletedOnUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task RejectFriendshipRequest_ShouldFail_WhenIsAlreadyRejected()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser("Friend", "Friendman");
        await Database.SaveChangesAsync();

        var friendshipRequest = Database.AddFriendshipRequest(user, friend, rejected: true);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(friend.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectFriendshipRequest(input: {{ 
                        friendshipRequestId: ""{friendshipRequest.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectFriendshipRequest", DomainErrors.FriendshipRequest.AlreadyRejected);

        var friendshipRequestDb = Database.DbContext.Set<FriendshipRequest>().First(x => x.UserId == user.Id && x.FriendId == friend.Id);
        friendshipRequestDb.Accepted.Should().BeFalse();
        friendshipRequestDb.Rejected.Should().BeTrue();
        friendshipRequestDb.CompletedOnUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task RejectFriendshipRequest_ShouldSucceed_AndNotCreateFriendship()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser("Friend", "Friendman");

        var friendshipRequest = Database.AddFriendshipRequest(user, friend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(friend.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectFriendshipRequest(input: {{ 
                        friendshipRequestId: ""{friendshipRequest.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("rejectFriendshipRequest");

        var friendshipRequestDb = Database.DbContext.Set<FriendshipRequest>().First(x => x.UserId == user.Id && x.FriendId == friend.Id);
        friendshipRequestDb.Accepted.Should().BeFalse();
        friendshipRequestDb.Rejected.Should().BeTrue();
        friendshipRequestDb.CompletedOnUtc.Should().NotBeNull();

        Database.DbContext.Set<Friendship>()
            .FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id)
            .Should().BeNull();
    }
}