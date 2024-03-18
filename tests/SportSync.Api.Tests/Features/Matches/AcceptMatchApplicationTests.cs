using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class AcceptMatchApplicationTests : IntegrationTest
{
    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenMatchApplicationNotFound()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptMatchApplication", DomainErrors.MatchApplication.NotFound);
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenUserIsNotAdmin()
    {
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent.Id, true);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
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
    public async Task AcceptMatchApplication_ShouldFail_WhenApplicationIsAlreadyAccepted()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent.Id, true);
        
        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();
        application.Accept(adminOnEvent, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptMatchApplication", DomainErrors.MatchApplication.AlreadyAccepted);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeTrue();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldFail_WhenApplicationIsAlreadyRejected()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent.Id, true);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();
        application.Reject(adminOnEvent, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("acceptMatchApplication", DomainErrors.MatchApplication.AlreadyRejected);
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeFalse();
        matchApplication.Rejected.Should().BeTrue();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }

    [Fact]
    public async Task AcceptMatchApplication_ShouldSucceed_WhenUserIsAdmin()
    {
        var completedTime = DateTime.UtcNow;
        DateTimeProviderMock.Setup(d => d.UtcNow).Returns(completedTime);
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var match = Database.AddMatch(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        match.AddPlayers(new List<Guid>() { user.Id });
        match.Announce(adminOnEvent.Id, true);

        var application = Database.AddMatchApplication(applicant, match);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var initialApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        initialApplication.Accepted.Should().BeFalse();
        initialApplication.Rejected.Should().BeFalse();
        initialApplication.CompletedOnUtc.Should().BeNull();
        initialApplication.CompletedByUserId.Should().BeNull();

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().BeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    acceptMatchApplication(input: {{ 
                        matchApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("acceptMatchApplication");
        var matchApplication = Database.DbContext.Set<MatchApplication>().First(x => x.MatchId == match.Id);
        matchApplication.Accepted.Should().BeTrue();
        matchApplication.Rejected.Should().BeFalse();
        matchApplication.CompletedOnUtc.Should().Be(completedTime);
        matchApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);

        Database.DbContext.Set<Player>()
            .FirstOrDefault(x => x.UserId == applicant.Id && x.MatchId == match.Id)
            .Should().NotBeNull();
    }
}