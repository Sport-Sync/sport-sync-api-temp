using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Users;

[Collection("IntegrationTests")]
public class RemoveFriendshipTests : IntegrationTest
{
    [Fact]
    public async Task RemoveFriendship_ShouldRemoveFriend_ByInvitedFriend()
    {
        var user = Database.AddUser(firstName: "invitor");
        var friend = Database.AddUser(firstName: "friend");

        user.AddFriend(friend);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        Database.DbContext.Set<Friendship>()
            .FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id).Should().NotBeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation{{
                    removeFriendship(input: {{friendId: ""{friend.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("removeFriendship");

        Database.DbContext.Set<Friendship>()
            .FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id).Should().BeNull();
    }

    [Fact]
    public async Task RemoveFriendship_ShouldRemoveFriend_ByInvitorUser()
    {
        var user = Database.AddUser(firstName: "invitor");
        var friend = Database.AddUser(firstName: "friend");

        user.AddFriend(friend);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(friend.Id);

        Database.DbContext.Set<Friendship>()
            .FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id).Should().NotBeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation{{
                    removeFriendship(input: {{friendId: ""{user.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("removeFriendship");

        Database.DbContext.Set<Friendship>()
            .FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id).Should().BeNull();
    }

    [Fact]
    public async Task RemoveFriendship_ShouldReturnFriendNotFound_WhenWrongFriendIdSent()
    {
        var user = Database.AddUser(firstName: "invitor");
        var friend = Database.AddUser(firstName: "friend");

        user.AddFriend(friend);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(friend.Id);

        Database.DbContext.Set<Friendship>()
            .FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id).Should().NotBeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation{{
                    removeFriendship(input: {{friendId: ""{friend.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("removeFriendship", DomainErrors.FriendshipRequest.FriendNotFound);

        Database.DbContext.Set<Friendship>()
            .FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id).Should().NotBeNull();
    }
}