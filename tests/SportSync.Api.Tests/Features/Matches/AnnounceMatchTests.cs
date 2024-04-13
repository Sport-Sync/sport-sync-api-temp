using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class AnnounceMatchTests : IntegrationTest
{
    [Fact]
    public async Task Announce_ShouldFail_WhenMatchNotFound()
    {
        var requestUser = Database.AddUser();
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{Guid.NewGuid()}"", playerLimit: 1, publicAnnouncement: true}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Match.NotFound);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task AnnounceByNonAdmin_ShouldSucceed_OnlyForFriendList(bool publicAnnouncement, bool shouldSucceed)
    {
        var requestUser = Database.AddUser();
        var admin = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { requestUser.Id });
        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: {publicAnnouncement.ToString().ToLower()}}}){{
                        eventName
                    }}
                }}"));
        if (shouldSucceed)
        {
            var matchResponse = result.ToResponseObject<MatchType>("announceMatch");
            matchResponse.Should().NotBeNull();

            var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
            announcement.UserId.Should().Be(requestUser.Id);
            announcement.AnnouncementType.Should().Be(MatchAnnouncementTypeEnum.FriendList);
        }
        else
        {
            result.ShouldHaveError(DomainErrors.User.Forbidden);
        }
    }

    [Fact]
    public async Task AnnounceToFriendList_ShouldCreateNotifications_ForFriendsThatAreNotPlayersAlready()
    {
        var requestUser = Database.AddUser();
        var friend = Database.AddUser();
        var friendPlayer = Database.AddUser();
        var match = Database.AddMatch(requestUser, startDate: DateTime.Today.AddDays(1));
        
        Database.AddFriendship(requestUser, friendPlayer);
        Database.AddFriendship(requestUser, friend);

        match.AddPlayers(new List<Guid>() { friendPlayer.Id });

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: false}}){{
                        eventName
                    }}
                }}"));

        var matchResponse = result.ToResponseObject<MatchType>("announceMatch");
        matchResponse.Should().NotBeNull();

        var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.UserId.Should().Be(requestUser.Id);
        announcement.AnnouncementType.Should().Be(MatchAnnouncementTypeEnum.FriendList);

        var notification = Database.DbContext.Set<Notification>().FirstOrDefault(n => n.UserId == friend.Id);
        notification.Should().NotBeNull();
        notification.Type.Should().Be(NotificationTypeEnum.MatchAnnouncedByFriend);
        notification.Completed.Should().BeFalse();
        notification.ResourceId.Should().Be(match.Id);

        Database.DbContext.Set<Notification>().FirstOrDefault(n => n.UserId == friendPlayer.Id).Should().BeNull();
    }

    [Fact]
    public async Task AnnouncePublicly_ShouldFail_WhenUserIsNotAdmin()
    {
        var requestUser = Database.AddUser();
        var admin = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { requestUser.Id });
        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: true}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.User.Forbidden);
    }

    [Theory]
    [InlineData(MatchStatusEnum.Finished, false)]
    [InlineData(MatchStatusEnum.Canceled, false)]
    [InlineData(MatchStatusEnum.Pending, true)]
    public async Task Announce_ShouldFail_WhenHasDoneStatus(MatchStatusEnum status, bool shouldSucceed)
    {
        var admin = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1), status: status);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: true}}){{
                        eventName
                    }}
                }}"));

        if (shouldSucceed)
        {
            result.ToResponseObject<MatchType>("announceMatch");
            var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
            announcement.UserId.Should().Be(admin.Id);
            announcement.AnnouncementType.Should().Be(MatchAnnouncementTypeEnum.Public);
        }
        else
        {
            result.ShouldHaveError(DomainErrors.Match.AlreadyFinished);
            Database.DbContext.Set<Match>().Find(match.Id).Status.Should().Be(status);
        }
    }

    [Theory]
    [InlineData(true, MatchAnnouncementTypeEnum.Public)]
    [InlineData(false, MatchAnnouncementTypeEnum.FriendList)]
    public async Task Announce_ShouldSucceed_AndSetProperStatus(bool publicly, MatchAnnouncementTypeEnum expectedType)
    {
        var admin = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: {publicly.ToString().ToLower()}}}){{
                        eventName
                    }}
                }}"));

        var matchResponse = result.ToResponseObject<MatchType>("announceMatch");
        matchResponse.Should().NotBeNull();

        var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.UserId.Should().Be(admin.Id);
        announcement.AnnouncementType.Should().Be(expectedType);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Announce_ShouldFail_WhenAlreadyIsPubliclyAnnounced(bool publicly)
    {
        var admin = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.Announce(admin, true, 3, string.Empty);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: {publicly.ToString().ToLower()}}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.MatchAnnouncement.AlreadyPubliclyAnnounced);

        var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.UserId.Should().Be(admin.Id);
    }

    [Fact]
    public async Task Announce_ShouldUpdatePlayersAnnouncerFlag_ForNonAdmin()
    {
        var admin = Database.AddUser();
        var user = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.AddPlayer(user.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var player = Database.DbContext.Set<Player>().Single(x => x.UserId == user.Id);
        player.HasAnnouncedMatch.Should().BeFalse();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);
        await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 2, publicAnnouncement: false}}){{
                        eventName
                    }}
                }}"));

        player = Database.DbContext.Set<Player>().Single(x => x.UserId == user.Id);
        player.HasAnnouncedMatch.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Announce_ShouldUpdatePlayersAnnouncerFlag_ForAdmin(bool publicly)
    {
        var admin = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        
        await Database.UnitOfWork.SaveChangesAsync();

        var player = Database.DbContext.Set<Player>().Single(x => x.UserId == admin.Id);
        player.HasAnnouncedMatch.Should().BeFalse();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 2, publicAnnouncement: {publicly.ToString().ToLower()}}}){{
                        eventName
                    }}
                }}"));

        player = Database.DbContext.Set<Player>().Single(x => x.UserId == admin.Id);
        player.HasAnnouncedMatch.Should().BeTrue();
    }

    [Fact]
    public async Task Announce_ShouldUpdatePrivateAnnouncement_WhenAnnouncingPublicly()
    {
        var admin = Database.AddUser();
        var user = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.AddPlayer(user.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 2, publicAnnouncement: false, description: ""We need some good players!!!""}}){{
                        eventName
                    }}
                }}"));

        var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.PlayerLimit.Should().Be(2);
        announcement.AnnouncementType.Should().Be(MatchAnnouncementTypeEnum.FriendList);
        announcement.Description.Should().Be("We need some good players!!!");

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result2 = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 5, publicAnnouncement: true, description: ""We are fine with bad ones as well.""}}){{
                        eventName
                    }}
                }}"));

        var matchResponse = result2.ToResponseObject<MatchType>("announceMatch");
        matchResponse.Should().NotBeNull();

        announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.PlayerLimit.Should().Be(5);
        announcement.AnnouncementType.Should().Be(MatchAnnouncementTypeEnum.Public);
        announcement.Description.Should().Be("We are fine with bad ones as well.");
    }

    [Theory]
    [InlineData(5, 1, true)]
    [InlineData(5, 4, true)]
    [InlineData(3, 2, true)]
    [InlineData(5, 5, false)]
    [InlineData(3, 3, false)]
    [InlineData(3, 4, false)]
    [InlineData(1, 2, false)]
    public async Task Announce_ShouldFail_WhenAnnouncingPubliclyOnExistingPrivateAnnouncementWithLimitLowerThanAlreadyAccepted(
        int limit, int alreadyAccepted, bool shouldSucceed)
    {
        var admin = Database.AddUser();
        var user = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.AddPlayer(user.Id);
        
        var announcmenet = match.Announce(user, false, limit, string.Empty);
        announcmenet.AcceptedPlayersCount = alreadyAccepted;

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: {limit}, publicAnnouncement: true, description: ""We are fine with bad ones as well.""}}){{
                        eventName
                    }}
                }}"));

        if (shouldSucceed)
        {
            var matchResponse = result.ToResponseObject<MatchType>("announceMatch");
            matchResponse.Should().NotBeNull();

            var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
            announcement.PlayerLimit.Should().Be(limit);
            announcement.AnnouncementType.Should().Be(MatchAnnouncementTypeEnum.Public);
            announcement.AcceptedPlayersCount.Should().Be(alreadyAccepted);
        }
        else
        {
            result.ShouldHaveError(DomainErrors.MatchAnnouncement.PlayerLimitLessThanAlreadyAccepted);

            var announcementFail = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
            announcementFail.PlayerLimit.Should().Be(limit);
            announcementFail.AnnouncementType.Should().Be(MatchAnnouncementTypeEnum.FriendList);
        }
    }

    [Fact]
    public async Task Announce_ShouldFail_WhenUserIsNotAPlayer()
    {
        var admin = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        var user = Database.AddUser("second", "user", "user@gmail.com");

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: false}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.MatchAnnouncement.UserIsNotPlayer);
    }

    [Fact]
    public async Task Announce_ShouldFail_WhenAlreadyAnnouncedBySameUser()
    {
        var admin = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.Announce(admin, false, 3, string.Empty);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: false}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.MatchAnnouncement.AlreadyAnnounced);

        var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.UserId.Should().Be(admin.Id);
    }

    [Fact]
    public async Task Announce_ShouldFail_WhenAlreadyAnnouncedPrivatelyByDifferentUser()
    {
        var admin = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        var user = Database.AddUser("second", "user", "user@gmail.com");
        
        match.AddPlayer(user.Id);
        match.Announce(user, false, 3, string.Empty);
        
        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", playerLimit: 1, publicAnnouncement: false}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.MatchAnnouncement.AlreadyAnnounced);
    }
}