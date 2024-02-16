using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Types;

namespace SportSync.Api.Tests.Features.Termins;

[Collection("IntegrationTests")]
public class AnnounceTerminTests : IntegrationTest
{
    [Fact]
    public async Task Announce_ShouldFail_WhenTerminNotFound()
    {
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceTermin(input: {{terminId: ""{Guid.NewGuid()}"", publicAnnouncement: true}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Termin.NotFound);
    }

    [Fact]
    public async Task Announce_ShouldFail_WhenUserIsNotAdmin()
    {
        var requestUser = Database.AddUser();
        var admin = Database.AddUser("second", "user", "user@gmail.com", "034234329");
        var termin = Database.AddTermin(admin, startDate: DateTime.Today.AddDays(1));
        termin.AddPlayers(new List<Guid>() { requestUser.Id });
        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceTermin(input: {{terminId: ""{termin.Id}"", publicAnnouncement: true}}){{
                        eventName
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.User.Forbidden);
    }

    [Theory]
    [InlineData(TerminStatus.Finished, false)]
    [InlineData(TerminStatus.Canceled, false)]
    [InlineData(TerminStatus.Pending, true)]
    public async Task Announce_ShouldFail_WhenHasDoneStatus(TerminStatus status, bool shouldSucceed)
    {
        var admin = Database.AddUser("second", "user", "user@gmail.com", "034234329");
        var termin = Database.AddTermin(admin, startDate: DateTime.Today.AddDays(1), status: status);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceTermin(input: {{terminId: ""{termin.Id}"", publicAnnouncement: true}}){{
                        eventName, status
                    }}
                }}"));

        if (shouldSucceed)
        {
            result.ToResponseObject<TerminType>("announceTermin");
            var announcement = Database.DbContext.Set<TerminAnnouncement>().Single(x => x.TerminId == termin.Id);
            announcement.UserId.Should().Be(admin.Id);
            announcement.AnnouncementType.Should().Be(TerminAnnouncementType.Public);
        }
        else
        {
            result.ShouldHaveError(DomainErrors.Termin.AlreadyFinished);
            Database.DbContext.Set<Termin>().Find(termin.Id).Status.Should().Be(status);
        }
    }

    [Theory]
    [InlineData(true, TerminAnnouncementType.Public)]
    [InlineData(false, TerminAnnouncementType.FriendList)]
    public async Task Announce_ShouldSucceed_AndSetProperStatus(bool publicly, TerminAnnouncementType expectedType)
    {
        var admin = Database.AddUser("second", "user", "user@gmail.com", "034234329");
        var termin = Database.AddTermin(admin, startDate: DateTime.Today.AddDays(1));

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceTermin(input: {{terminId: ""{termin.Id}"", publicAnnouncement: {publicly.ToString().ToLower()}}}){{
                        eventName, status
                    }}
                }}"));

        var terminResponse = result.ToResponseObject<TerminType>("announceTermin");
        terminResponse.Should().NotBeNull();

        var announcement = Database.DbContext.Set<TerminAnnouncement>().Single(x => x.TerminId == termin.Id);
        announcement.UserId.Should().Be(admin.Id);
        announcement.AnnouncementType.Should().Be(expectedType);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Announce_ShouldFail_WhenAlreadyIsPubliclyAnnounced(bool publicly)
    {
        var admin = Database.AddUser();
        var termin = Database.AddTermin(admin, startDate: DateTime.Today.AddDays(1));
        termin.Announce(admin.Id, true);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceTermin(input: {{terminId: ""{termin.Id}"", publicAnnouncement: {publicly.ToString().ToLower()}}}){{
                        eventName, status
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.TerminAnnouncement.AlreadyPubliclyAnnounced);

        var announcement = Database.DbContext.Set<TerminAnnouncement>().Single(x => x.TerminId == termin.Id);
        announcement.UserId.Should().Be(admin.Id);
    }

    [Fact]
    public async Task Announce_ShouldRemovePrivateAnnouncement_WhenAnnouncingPublicly()
    {
        var admin = Database.AddUser();
        var user = Database.AddUser("second", "user", "user@gmail.com", "034234329");
        var termin = Database.AddTermin(admin, startDate: DateTime.Today.AddDays(1));
        termin.Announce(admin.Id, false);
        termin.Announce(user.Id, false);
        
        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceTermin(input: {{terminId: ""{termin.Id}"", publicAnnouncement: true}}){{
                        eventName, status
                    }}
                }}"));

        var terminResponse = result.ToResponseObject<TerminType>("announceTermin");
        terminResponse.Should().NotBeNull();

        var announcements = Database.DbContext.Set<TerminAnnouncement>().Where(x => x.TerminId == termin.Id);
        announcements.Count().Should().Be(1);
        announcements.Single().AnnouncementType.Should().Be(TerminAnnouncementType.Public);
    }

    [Fact]
    public async Task Announce_ShouldFail_WhenAlreadyAnnouncedBySameUser()
    {
        var admin = Database.AddUser();
        var termin = Database.AddTermin(admin, startDate: DateTime.Today.AddDays(1));
        termin.Announce(admin.Id, false);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceTermin(input: {{terminId: ""{termin.Id}"", publicAnnouncement: false}}){{
                        eventName, status
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.TerminAnnouncement.AlreadyAnnouncedBySameUser);

        var announcement = Database.DbContext.Set<TerminAnnouncement>().Single(x => x.TerminId == termin.Id);
        announcement.UserId.Should().Be(admin.Id);
    }

    [Fact]
    public async Task Announce_ShouldSucceed_WhenAlreadyAnnouncedByDifferentUser()
    {
        var admin = Database.AddUser();
        var termin = Database.AddTermin(admin, startDate: DateTime.Today.AddDays(1));
        var user = Database.AddUser("second", "user", "user@gmail.com", "034234329");
        termin.Announce(user.Id, false);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    announceTermin(input: {{terminId: ""{termin.Id}"", publicAnnouncement: false}}){{
                        eventName, status
                    }}
                }}"));

        var terminResponse = result.ToResponseObject<TerminType>("announceTermin");
        terminResponse.Should().NotBeNull();

        var announcements = Database.DbContext.Set<TerminAnnouncement>().Where(x => x.TerminId == termin.Id);
        announcements.Count().Should().Be(2);

        announcements.Single(x => x.UserId == user.Id).AnnouncementType.Should().Be(TerminAnnouncementType.FriendList);
        announcements.Single(x => x.UserId == admin.Id).AnnouncementType.Should().Be(TerminAnnouncementType.FriendList);
    }
}