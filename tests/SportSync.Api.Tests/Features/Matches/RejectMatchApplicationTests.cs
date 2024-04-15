using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class RejectMatchApplicationTests : IntegrationTest
{
    [Fact]
    public async Task RejectMatchApplication_ShouldFail_WhenMatchApplicationNotFound()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectMatchApplication(input: {{ 
                        matchApplicationId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectMatchApplication", DomainErrors.MatchApplication.NotFound);
    }

    [Fact]
    public async Task RejectMatchApplication_ShouldFail_WhenUserIsNotAdmin()
    {
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldHaveError(DomainErrors.User.Forbidden);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().BeNull();
        matchApplication.CompletedByUserId.Should().BeNull();
    }

    [Fact]
    public async Task RejectMatchApplication_ShouldFail_WhenApplicationIsAlreadyAccepted()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);
        
        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();
        application.Accept(adminOnEvent, match, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectMatchApplication", DomainErrors.MatchApplication.AlreadyAccepted);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeTrue();
        matchApplication.Canceled.Should().BeFalse();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }

    [Fact]
    public async Task RejectMatchApplication_ShouldFail_WhenApplicationIsAlreadyRejected()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();
        application.Reject(adminOnEvent, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectMatchApplication", DomainErrors.MatchApplication.AlreadyRejected);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Canceled.Should().BeFalse();
        matchApplication.Rejected.Should().BeTrue();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }

    [Fact]
    public async Task RejectMatchApplication_ShouldFail_WhenApplicationIsAlreadyCanceled()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();
        application.Cancel(applicant, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectMatchApplication", DomainErrors.MatchApplication.AlreadyCanceled);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.Canceled.Should().BeTrue();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(applicant.Id);
    }

    [Fact]
    public async Task RejectMatchApplication_ShouldSucceed_WhenUserIsAdmin()
    {
        var completedTime = DateTime.UtcNow;
        DateTimeProviderMock.Setup(d => d.UtcNow).Returns(completedTime);
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent, true, 3, string.Empty);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var initialApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        initialApplication.Accepted.Should().BeFalse();
        initialApplication.Rejected.Should().BeFalse();
        initialApplication.CompletedOnUtc.Should().BeNull();
        initialApplication.CompletedByUserId.Should().BeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("rejectMatchApplication");
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Rejected.Should().BeTrue();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }
}