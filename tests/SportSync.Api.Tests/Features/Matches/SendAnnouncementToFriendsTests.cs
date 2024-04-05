using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class SendAnnouncementToFriendsTests : IntegrationTest
{
    [Fact]
    public async Task SendAnnouncementToFriends_ShouldFail_WhenMatchDoesntExists()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendAnnouncementToFriends(input: {{ 
                        matchId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("sendAnnouncementToFriends", DomainErrors.Match.NotFound);
    }

    [Fact]
    public async Task SendAnnouncementToFriends_ShouldFail_WhenAnnouncementDoesntExists()
    {
        var user = Database.AddUser();
        var match = Database.AddMatch(user);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendAnnouncementToFriends(input: {{ 
                        matchId: ""{match.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("sendAnnouncementToFriends", DomainErrors.MatchAnnouncement.NotAnnounced);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SendAnnouncementToFriends_ShouldFail_WhenUserIsNotPlayer(bool publiclyAnnounced)
    {
        var user = Database.AddUser();
        var nonPlayer = Database.AddUser();
        var match = Database.AddMatch(user, startDate: DateTime.Today.AddDays(1));
        match.Announce(user, publiclyAnnounced, 2);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(nonPlayer.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendAnnouncementToFriends(input: {{ 
                        matchId: ""{match.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("sendAnnouncementToFriends", DomainErrors.MatchAnnouncement.UserIsNotPlayer);
    }

    [Fact]
    public async Task SendAnnouncementToFriends_Should_CreateNotificationsForFriends()
    {
        var admin = Database.AddUser();
        var user = Database.AddUser();
        var friend1 = Database.AddUser();
        var friend2 = Database.AddUser();
        var friend3 = Database.AddUser();
        var adminFriend = Database.AddUser();

        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.AddPlayer(user.Id);
        match.Announce(admin, true, 2);

        Database.AddFriendship(user, friend1);
        Database.AddFriendship(user, friend2);
        Database.AddFriendship(friend3, user);
        Database.AddFriendship(admin, adminFriend);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendAnnouncementToFriends(input: {{ 
                        matchId: ""{match.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("sendAnnouncementToFriends");

        var notifications = Database.DbContext.Set<Notification>().ToList();
        notifications.Count.Should().Be(3);
        
        notifications.FirstOrDefault(x => x.UserId == friend1.Id).Should().NotBeNull();
        notifications.FirstOrDefault(x => x.UserId == friend1.Id).Type.Should().Be(NotificationTypeEnum.MatchAnnouncedByFriend);

        notifications.FirstOrDefault(x => x.UserId == friend2.Id).Should().NotBeNull();
        notifications.FirstOrDefault(x => x.UserId == friend2.Id).Type.Should().Be(NotificationTypeEnum.MatchAnnouncedByFriend);

        notifications.FirstOrDefault(x => x.UserId == friend3.Id).Should().NotBeNull();
        notifications.FirstOrDefault(x => x.UserId == friend3.Id).Type.Should().Be(NotificationTypeEnum.MatchAnnouncedByFriend);
    }

    [Fact]
    public async Task SendAnnouncementToFriends_Should_SetHasAnnouncedMatchFlag()
    {
        var admin = Database.AddUser();
        var user = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.AddPlayer(user.Id);
        match.Announce(admin, true, 2);

        await Database.SaveChangesAsync();

        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id).HasAnnouncedMatch.Should().BeFalse();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    sendAnnouncementToFriends(input: {{ 
                        matchId: ""{match.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("sendAnnouncementToFriends");

        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id).HasAnnouncedMatch.Should().BeTrue();
    }
}