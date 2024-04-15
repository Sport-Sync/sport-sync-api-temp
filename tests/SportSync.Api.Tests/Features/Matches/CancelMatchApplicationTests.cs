using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class CancelMatchApplicationTests : IntegrationTest
{
    [Fact]
    public async Task CancelApplication_ShouldFail_WhenMatchApplicationNotFound()
    {
        var requestUser = Database.AddUser();
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchApplication(input: {{matchApplicationId: ""{Guid.NewGuid()}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchApplication", DomainErrors.MatchApplication.NotFound);
    }

    [Fact]
    public async Task CancelApplication_ShouldFail_WhenMatchApplicationAlreadyAccepted()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        
        match.Announce(admin, true, 1);
        
        var application = Database.AddMatchApplication(requestUser, match);

        await Database.UnitOfWork.SaveChangesAsync();

        application.Accept(admin, match, DateTime.UtcNow);
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchApplication(input: {{matchApplicationId: ""{application.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchApplication", DomainErrors.MatchApplication.AlreadyAccepted);
    }

    [Fact]
    public async Task CancelApplication_ShouldFail_WhenMatchApplicationAlreadyRejected()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        
        match.Announce(admin, true, 1);
        
        var application = Database.AddMatchApplication(requestUser, match);

        await Database.UnitOfWork.SaveChangesAsync();

        application.Reject(admin, DateTime.UtcNow);
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchApplication(input: {{matchApplicationId: ""{application.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchApplication", DomainErrors.MatchApplication.AlreadyRejected);
    }

    [Fact]
    public async Task CancelApplication_ShouldFail_WhenMatchApplicationAlreadyCanceled()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));

        match.Announce(admin, true, 1);

        var application = Database.AddMatchApplication(requestUser, match);

        await Database.UnitOfWork.SaveChangesAsync();

        application.Cancel(requestUser, DateTime.UtcNow);
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        await Database.UnitOfWork.SaveChangesAsync();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchApplication(input: {{matchApplicationId: ""{application.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchApplication", DomainErrors.MatchApplication.AlreadyCanceled);
    }

    [Fact]
    public async Task CancelApplication_ShouldFail_WhenMatchUserIsNotApplier()
    {
        var admin = Database.AddUser();
        var applierUser = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.Announce(admin, true, 1);
        
        var application = Database.AddMatchApplication(applierUser, match);

        await Database.UnitOfWork.SaveChangesAsync();
        
        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);
        
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchApplication(input: {{matchApplicationId: ""{application.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeFailureResult("cancelMatchApplication", DomainErrors.User.Forbidden);
    }

    [Fact]
    public async Task CancelApplication_Should_DeleteAdminNotification()
    {
        var admin = Database.AddUser();
        var requestUser = Database.AddUser();
        var match = Database.AddMatch(admin, startDate: DateTime.Now.AddDays(1));
        match.Announce(admin, true, 1);

        await Database.UnitOfWork.SaveChangesAsync();

        var application = match.ApplyForPlaying(requestUser);

        Database.DbContext.Set<MatchApplication>().Add(application.Value);

        await Database.UnitOfWork.SaveChangesAsync();

        var notification = Database.DbContext.Set<Notification>().FirstOrDefault(x => x.Type == NotificationTypeEnum.MatchApplicationReceived);
        notification.Should().NotBeNull();
        notification.UserId.Should().Be(admin.Id);

        UserIdentifierMock.Setup(x => x.UserId).Returns(requestUser.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    cancelMatchApplication(input: {{matchApplicationId: ""{application.Value.Id}""}}){{
                        isSuccess, isFailure, error{{ message, code }}
                    }}
                }}"));

        result.ShouldBeSuccessResult("cancelMatchApplication");

        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.Canceled.Should().BeTrue();
        matchApplication.CompletedOnUtc.Should().NotBeNull();
        matchApplication.CompletedByUserId.Should().Be(requestUser.Id);

        Database.DbContext.Set<Notification>().FirstOrDefault(x => x.Type == NotificationTypeEnum.MatchApplicationReceived).Should().BeNull();
        Database.DbContext.Set<Notification>().FirstOrDefault(x => x.UserId == admin.Id).Should().BeNull();
    }
}