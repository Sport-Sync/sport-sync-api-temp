using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class AddFriendToMatchTests : IntegrationTest
{
    [Fact]
    public async Task AddFriendToMatch_ShouldFail_WhenMatchNotFound()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    addFriendToMatch(input: {{ 
                        matchId: ""{Guid.NewGuid()}"", friendId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("addFriendToMatch", DomainErrors.Match.NotFound);
    }

    [Fact]
    public async Task AddFriendToMatch_ShouldFail_WhenUserIsNotPlayer()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser();
        var notPlayer = Database.AddUser();
        var match = Database.AddMatch(user);

        notPlayer.AddFriend(friend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(notPlayer.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    addFriendToMatch(input: {{ 
                        matchId: ""{match.Id}"", friendId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("addFriendToMatch", DomainErrors.Match.PlayerNotFound);
    }

    [Fact]
    public async Task AddFriendToMatch_ShouldFail_WhenFriendNotFound()
    {
        var user = Database.AddUser();
        var match = Database.AddMatch(user);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    addFriendToMatch(input: {{ 
                        matchId: ""{match.Id}"", friendId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("addFriendToMatch", DomainErrors.User.NotFriends);
    }

    [Fact]
    public async Task AddFriendToMatch_ShouldFail_WhenNotFriends()
    {
        var user = Database.AddUser();
        var notFriend = Database.AddUser();
        var match = Database.AddMatch(user);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    addFriendToMatch(input: {{ 
                        matchId: ""{match.Id}"", friendId: ""{notFriend.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("addFriendToMatch", DomainErrors.User.NotFriends);
    }

    [Fact]
    public async Task AddFriendToMatch_ShouldFail_WhenFriendIsAlreadyPlayer()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser();
        var match = Database.AddMatch(user);

        user.AddFriend(friend);
        match.AddPlayer(friend.Id);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    addFriendToMatch(input: {{ 
                        matchId: ""{match.Id}"", friendId: ""{friend.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("addFriendToMatch", DomainErrors.Match.AlreadyPlayer);
    }

    [Theory]
    [InlineData(MatchStatusEnum.InProgress, false)]
    [InlineData(MatchStatusEnum.Canceled, false)]
    [InlineData(MatchStatusEnum.Finished, false)]
    [InlineData(MatchStatusEnum.Pending, true)]
    public async Task AddFriendToMatch_ShouldFail_WhenMatchIsNotPending(MatchStatusEnum status, bool shouldSucceed)
    {
        var user = Database.AddUser();
        var friend = Database.AddUser();
        var match = Database.AddMatch(user, status: status);

        user.AddFriend(friend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    addFriendToMatch(input: {{ 
                        matchId: ""{match.Id}"", friendId: ""{friend.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        if (shouldSucceed)
        {
            result.ShouldBeSuccessResult("addFriendToMatch");

            var players = Database.DbContext.Set<Player>().Where(p => p.MatchId == match.Id).ToList();
            players.Count.Should().Be(2);

            var newPlayer = players.First(x => x.UserId == friend.Id);
            newPlayer.Attending.Should().BeNull();
        }
        else
        {
            var error = status.ToError();
            result.ShouldBeFailureResult("addFriendToMatch", error);
        }
    }

    [Fact]
    public async Task AddFriendToMatch_Should_AddPlayerToMatch()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser();
        var match = Database.AddMatch(user);

        user.AddFriend(friend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    addFriendToMatch(input: {{ 
                        matchId: ""{match.Id}"", friendId: ""{friend.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("addFriendToMatch");

        var players = Database.DbContext.Set<Player>().Where(p => p.MatchId == match.Id).ToList();
        players.Count.Should().Be(2);

        var newPlayer = players.First(x => x.UserId == friend.Id);
        newPlayer.Attending.Should().BeNull();
    }

    [Fact]
    public async Task AddFriendToMatch_ShouldCreateNotification_ForNewPlayer()
    {
        var user = Database.AddUser();
        var friend = Database.AddUser();
        var match = Database.AddMatch(user);

        user.AddFriend(friend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    addFriendToMatch(input: {{ 
                        matchId: ""{match.Id}"", friendId: ""{friend.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("addFriendToMatch");

        var players = Database.DbContext.Set<Player>().Where(p => p.MatchId == match.Id).ToList();
        players.Count.Should().Be(2);

        var notification = Database.DbContext.Set<Notification>().Single(x => x.UserId == friend.Id);
        notification.Completed.Should().BeFalse();
        notification.ResourceId.Should().Be(match.Id);
        notification.Type.Should().Be(NotificationTypeEnum.AddedToMatchByFriend);
    }
}