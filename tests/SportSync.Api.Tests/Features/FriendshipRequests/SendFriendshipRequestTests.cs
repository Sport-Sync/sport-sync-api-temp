using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.FriendshipRequests;

[Collection("IntegrationTests")]
public class SendFriendshipRequestTests : IntegrationTest
{
    [Fact]
    public async Task SendFriendshipRequest_ShouldFail_WhenUserIdAndFriendIdAreSame()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendFriendshipRequest(input: {{ 
                        userId: ""{user.Id}"", friendIds: [""{user.Id}""] }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("sendFriendshipRequest", DomainErrors.FriendshipRequest.InvalidSameUserId);
        Database.DbContext.Set<FriendshipRequest>().FirstOrDefault(x => x.UserId == user.Id && x.FriendId == user.Id).Should().BeNull();
    }

    [Fact]
    public async Task SendFriendshipRequest_ShouldFail_WhenFriendshipRequestExists()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser("Mario", "Marić", "mail@mail.com");
        Database.AddFriendshipRequest(user, friend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendFriendshipRequest(input: {{ 
                        userId: ""{user.Id}"", friendIds: [""{friend.Id}""] }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("sendFriendshipRequest", DomainErrors.FriendshipRequest.PendingFriendshipRequest);
    }

    [Fact]
    public async Task SendFriendshipRequest_ShouldFail_WhenAlreadyFriends()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser("Mario", "Marić", "mail@mail.com");
        Database.AddFriendship(user, friend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendFriendshipRequest(input: {{ 
                        userId: ""{user.Id}"", friendIds:  [""{friend.Id}""] }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("sendFriendshipRequest", DomainErrors.FriendshipRequest.AlreadyFriends);

        Database.DbContext.Set<FriendshipRequest>().FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id).Should().BeNull();
    }

    [Fact]
    public async Task SendFriendshipRequest_ShouldSucceed()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser("Mario", "Marić", "mail@mail.com");
        var friend2 = Database.AddUser("Friend", "Second", "friend@mail.com");

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendFriendshipRequest(input: {{ 
                        userId: ""{user.Id}"", friendIds:  [""{friend.Id}"", ""{friend2.Id}""] }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("sendFriendshipRequest");

        var friendshipRequest = Database.DbContext.Set<FriendshipRequest>().First(x => x.UserId == user.Id && x.FriendId == friend.Id);
        friendshipRequest.Accepted.Should().BeFalse();
        friendshipRequest.Rejected.Should().BeFalse();
        friendshipRequest.CompletedOnUtc.Should().BeNull();

        var friendshipRequest2 = Database.DbContext.Set<FriendshipRequest>().First(x => x.UserId == user.Id && x.FriendId == friend2.Id);
        friendshipRequest2.Accepted.Should().BeFalse();
        friendshipRequest2.Rejected.Should().BeFalse();
        friendshipRequest2.CompletedOnUtc.Should().BeNull();
    }

    [Fact]
    public async Task SendFriendshipRequest_ShouldSucceed_EvenWhenOneFailed()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser("Mario", "Marić", "mail@mail.com");
        var friend2 = Database.AddUser("Friend", "Second", "friend@mail.com");

        Database.AddFriendship(user, friend);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendFriendshipRequest(input: {{ 
                        userId: ""{user.Id}"", friendIds:  [""{friend.Id}"", ""{friend2.Id}""] }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("sendFriendshipRequest");

        Database.DbContext.Set<FriendshipRequest>().FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id).Should().BeNull();

        var friendshipRequest2 = Database.DbContext.Set<FriendshipRequest>().First(x => x.UserId == user.Id && x.FriendId == friend2.Id);
        friendshipRequest2.Accepted.Should().BeFalse();
        friendshipRequest2.Rejected.Should().BeFalse();
        friendshipRequest2.CompletedOnUtc.Should().BeNull();
    }
}