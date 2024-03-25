using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class SendMatchApplicationTests : IntegrationTest
{
    [Fact]
    public async Task SendMatchApplication_ShouldFail_WhenUserIsAlreadyPlayer()
    {
        var admin = Database.AddUser();
        var player = Database.AddUser("Player");

        var match = Database.AddMatch(admin, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { player.Id });
        match.Announce(admin, true);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(player.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendMatchApplication(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("sendMatchApplication", DomainErrors.MatchApplication.AlreadyPlayer);

        var application = Database.DbContext.Set<MatchApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == player.Id && x.MatchId == match.Id);

        application.Should().BeNull();
    }

    [Fact]
    public async Task SendMatchApplication_ShouldFail_WhenMatchIsNotAnnounced()
    {
        var admin = Database.AddUser();
        var player = Database.AddUser("Player");

        var match = Database.AddMatch(admin);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(player.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendMatchApplication(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("sendMatchApplication", DomainErrors.MatchApplication.NotAnnounced);

        var application = Database.DbContext.Set<MatchApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == player.Id && x.MatchId == match.Id);

        application.Should().BeNull();
    }

    [Fact]
    public async Task SendMatchApplication_ShouldFail_WhenUserIsNotOnFriendList()
    {
        var admin = Database.AddUser();
        var player = Database.AddUser("Player");

        var match = Database.AddMatch(admin, status: MatchStatus.Pending, startDate: DateTime.Today.AddDays(1));
        match.Announce(admin, false);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(player.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendMatchApplication(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("sendMatchApplication", DomainErrors.MatchApplication.NotOnFriendList);

        var application = Database.DbContext.Set<MatchApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == player.Id && x.MatchId == match.Id);

        application.Should().BeNull();
    }

    [Fact]
    public async Task SendMatchApplication_ShouldSucceed_WhenPublicAnnouncement()
    {
        var admin = Database.AddUser();
        var applicant = Database.AddUser("Player");

        var match = Database.AddMatch(admin, status: MatchStatus.Pending, startDate: DateTime.Today.AddDays(1));
        match.Announce(admin, true);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(applicant.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendMatchApplication(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("sendMatchApplication");

        var application = Database.DbContext.Set<MatchApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == applicant.Id && x.MatchId == match.Id);

        application.Should().NotBeNull();
        application.Rejected.Should().BeFalse();
        application.Accepted.Should().BeFalse();
        application.CompletedOnUtc.Should().BeNull();
    }

    [Fact]
    public async Task SendMatchApplication_ShouldSucceed_WhenPrivateAnnouncementAndUserIsFriend()
    {
        var admin = Database.AddUser();
        var applicant = Database.AddUser("Player");

        var match = Database.AddMatch(admin, status: MatchStatus.Pending, startDate: DateTime.Today.AddDays(1));
        Database.AddFriendship(admin, applicant);
        match.Announce(admin, false);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(applicant.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendMatchApplication(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("sendMatchApplication");

        var application = Database.DbContext.Set<MatchApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == applicant.Id && x.MatchId == match.Id);

        application.Should().NotBeNull();
        application.Rejected.Should().BeFalse();
        application.Accepted.Should().BeFalse();
        application.CompletedOnUtc.Should().BeNull();
    }

    [Fact]
    public async Task SendMatchApplication_ShouldCreateNotification_ForAllAdmins()
    {
        var admin1 = Database.AddUser();
        var admin2 = Database.AddUser();
        var applicant = Database.AddUser("Player");

        var match = Database.AddMatch(admin1, status: MatchStatus.Pending, startDate: DateTime.Today.AddDays(1));

        match.Announce(admin1, true);

        await Database.SaveChangesAsync();

        var @event = Database.DbContext.Set<Event>().FirstOrDefault();
        @event.AddMembers(admin2.Id);
        @event.MakeAdmin(admin2);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(applicant.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    sendMatchApplication(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("sendMatchApplication");

        var application = Database.DbContext.Set<MatchApplication>()
            .FirstOrDefault(x => x.AppliedByUserId == applicant.Id && x.MatchId == match.Id);

        application.Should().NotBeNull();
        application.Rejected.Should().BeFalse();
        application.Accepted.Should().BeFalse();
        application.CompletedOnUtc.Should().BeNull();

        var notifications = Database.DbContext.Set<Notification>().Where(x => x.ResourceId == match.Id);
        notifications.Count().Should().Be(2);

        notifications.FirstOrDefault(x => x.UserId == admin1.Id && x.Type == NotificationTypeEnum.MatchApplicationReceived).Should().NotBeNull();
        notifications.FirstOrDefault(x => x.UserId == admin2.Id && x.Type == NotificationTypeEnum.MatchApplicationReceived).Should().NotBeNull();

        foreach (var notification in notifications)
        {
            notification.Completed.Should().BeFalse();
            notification.CompletedOnUtc.Should().BeNull();
            notification.ResourceId.Should().Be(match.Id);
            notification.Type.Should().Be(NotificationTypeEnum.MatchApplicationReceived);
            notification.ContentData.Data.Should().BeEquivalentTo(applicant.FullName, @event.Name, match.Date.ToShortDateString());
        }
    }
}