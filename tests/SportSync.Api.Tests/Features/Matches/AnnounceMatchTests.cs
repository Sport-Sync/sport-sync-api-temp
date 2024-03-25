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
                    announceMatch(input: {{matchId: ""{Guid.NewGuid()}"", publicAnnouncement: true}}){{
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
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: {publicAnnouncement.ToString().ToLower()}}}){{
                        eventName
                    }}
                }}"));
        if (shouldSucceed)
        {
            var matchResponse = result.ToResponseObject<MatchType>("announceMatch");
            matchResponse.Should().NotBeNull();

            var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
            announcement.UserId.Should().Be(requestUser.Id);
            announcement.AnnouncementType.Should().Be(MatchAnnouncementType.FriendList);
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
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: false}}){{
                        eventName
                    }}
                }}"));

        var matchResponse = result.ToResponseObject<MatchType>("announceMatch");
        matchResponse.Should().NotBeNull();

        var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.UserId.Should().Be(requestUser.Id);
        announcement.AnnouncementType.Should().Be(MatchAnnouncementType.FriendList);

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
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: true}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.User.Forbidden);
    }

    [Theory]
    [InlineData(MatchStatus.Finished, false)]
    [InlineData(MatchStatus.Canceled, false)]
    [InlineData(MatchStatus.Pending, true)]
    public async Task Announce_ShouldFail_WhenHasDoneStatus(MatchStatus status, bool shouldSucceed)
    {
        var admin = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1), status: status);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: true}}){{
                        eventName, status
                    }}
                }}"));

        if (shouldSucceed)
        {
            result.ToResponseObject<MatchType>("announceMatch");
            var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
            announcement.UserId.Should().Be(admin.Id);
            announcement.AnnouncementType.Should().Be(MatchAnnouncementType.Public);
        }
        else
        {
            result.ShouldHaveError(DomainErrors.Match.AlreadyFinished);
            Database.DbContext.Set<Match>().Find(match.Id).Status.Should().Be(status);
        }
    }

    [Theory]
    [InlineData(true, MatchAnnouncementType.Public)]
    [InlineData(false, MatchAnnouncementType.FriendList)]
    public async Task Announce_ShouldSucceed_AndSetProperStatus(bool publicly, MatchAnnouncementType expectedType)
    {
        var admin = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: {publicly.ToString().ToLower()}}}){{
                        eventName, status
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
        match.Announce(admin, true);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: {publicly.ToString().ToLower()}}}){{
                        eventName, status
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.MatchAnnouncement.AlreadyPubliclyAnnounced);

        var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.UserId.Should().Be(admin.Id);
    }

    [Fact]
    public async Task Announce_ShouldRemovePrivateAnnouncement_WhenAnnouncingPublicly()
    {
        var admin = Database.AddUser();
        var user = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.Announce(admin, false);
        match.Announce(user, false);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: true}}){{
                        eventName, status
                    }}
                }}"));

        var matchResponse = result.ToResponseObject<MatchType>("announceMatch");
        matchResponse.Should().NotBeNull();

        var announcements = Database.DbContext.Set<MatchAnnouncement>().Where(x => x.MatchId == match.Id);
        announcements.Count().Should().Be(1);
        announcements.Single().AnnouncementType.Should().Be(MatchAnnouncementType.Public);
    }

    [Fact]
    public async Task Announce_ShouldFail_WhenAlreadyAnnouncedBySameUser()
    {
        var admin = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.Announce(admin, false);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: false}}){{
                        eventName, status
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.MatchAnnouncement.AlreadyAnnouncedBySameUser);

        var announcement = Database.DbContext.Set<MatchAnnouncement>().Single(x => x.MatchId == match.Id);
        announcement.UserId.Should().Be(admin.Id);
    }

    [Fact]
    public async Task Announce_ShouldSucceed_WhenAlreadyAnnouncedByDifferentUser()
    {
        var admin = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        var user = Database.AddUser("second", "user", "user@gmail.com");
        match.Announce(user, false);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceMatch(input: {{matchId: ""{match.Id}"", publicAnnouncement: false}}){{
                        eventName, status
                    }}
                }}"));

        var matchResponse = result.ToResponseObject<MatchType>("announceMatch");
        matchResponse.Should().NotBeNull();

        var announcements = Database.DbContext.Set<MatchAnnouncement>().Where(x => x.MatchId == match.Id);
        announcements.Count().Should().Be(2);

        announcements.Single(x => x.UserId == user.Id).AnnouncementType.Should().Be(MatchAnnouncementType.FriendList);
        announcements.Single(x => x.UserId == admin.Id).AnnouncementType.Should().Be(MatchAnnouncementType.FriendList);
    }
}