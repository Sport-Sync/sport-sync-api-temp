using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class CancelMatchAnnouncementTests : IntegrationTest
{
    [Fact]
    public async Task CancelAnnouncement_ShouldFail_WhenMatchNotFound()
    {
        var requestUser = Database.AddUser();
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{Guid.NewGuid()}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchAnnouncement", DomainErrors.Match.NotFound);
    }

    [Fact]
    public async Task CancelAnnouncement_ShouldFail_WhenMatchNotAnnounced()
    {
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(requestUser);
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchAnnouncement", DomainErrors.MatchAnnouncement.NotAnnounced);
    }

    [Fact]
    public async Task CancelAnnouncement_ShouldFail_WhenUserIsNotPlayer()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.Announce(admin, true, 3);
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchAnnouncement", DomainErrors.MatchAnnouncement.UserIsNotPlayer);
    }

    [Fact]
    public async Task CancelPrivateAnnouncement_ShouldFail_WhenUserIsNotAnnouncerAndNotAdmin()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.AddPlayer(requestUser.Id);
        match.Announce(admin, false, 3);

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchAnnouncement", DomainErrors.User.Forbidden);
    }

    [Fact]
    public async Task CancelPublicAnnouncement_ShouldFail_WhenUserIsNotAdmin()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.AddPlayer(requestUser.Id);
        match.Announce(admin, true, 3);

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchAnnouncement", DomainErrors.User.Forbidden);
    }

    [Fact]
    public async Task CancelPrivateAnnouncement_ShouldSucceed_WhenUserIsAnnouncerButNotAdmin()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.AddPlayer(requestUser.Id);
        match.Announce(requestUser, false, 3);

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("cancelMatchAnnouncement");
    }

    [Fact]
    public async Task CancelPrivateAnnouncement_ShouldSucceed_WhenUserIsAdminButNotAnnouncer()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.AddPlayer(requestUser.Id);
        match.Announce(requestUser, false, 3);

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("cancelMatchAnnouncement");
    }

    [Fact]
    public async Task CancelPrivateAnnouncement_ShouldSucceed_WhenUserIsAdminAndAnnouncer()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.AddPlayer(requestUser.Id);
        match.Announce(admin, false, 3);

        UserIdentifierMock.Setup(x => x.UserId).Returns(admin.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("cancelMatchAnnouncement");
    }

    [Fact]
    public async Task CancelPublicAnnouncement_ShouldSucceed_WhenUserIsAdminAndNotAnnouncer()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.AddPlayer(requestUser.Id);
        
        match.Announce(admin, true, 3);

        await Database.UnitOfWork.SaveChangesAsync();

        var ev = Database.DbContext.Set<Event>().Single(x => x.Id == match.EventId);
        ev.AddMembers(requestUser.Id);
        ev.MakeAdmin(requestUser);
        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("cancelMatchAnnouncement");
    }

    [Fact]
    public async Task CancelPublicAnnouncement_ShouldSucceed_WhenUserIsAdminAndAnnouncer()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.AddPlayer(requestUser.Id);


        await Database.UnitOfWork.SaveChangesAsync();

        var ev = Database.DbContext.Set<Event>().Single(x => x.Id == match.EventId);
        ev.AddMembers(requestUser.Id);
        ev.MakeAdmin(requestUser);
        match.Announce(requestUser, true, 3);
        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchAnnouncement(input: {{matchId: ""{match.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("cancelMatchAnnouncement");
    }
}