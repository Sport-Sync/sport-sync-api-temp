using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Termins;

[Collection("IntegrationTests")]
public class RejectTerminApplicationTests : IntegrationTest
{
    [Fact]
    public async Task RejectTerminApplication_ShouldFail_WhenTerminApplicationNotFound()
    {
        var user = Database.AddUser();
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectTerminApplication(input: {{ 
                        terminApplicationId: ""{Guid.NewGuid()}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectTerminApplication", DomainErrors.TerminApplication.NotFound);
    }

    [Fact]
    public async Task RejectTerminApplication_ShouldFail_WhenUserIsNotAdmin()
    {
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var termin = Database.AddTermin(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        termin.AddPlayers(new List<Guid>() { user.Id });
        termin.Announce(adminOnEvent.Id, true);

        var application = Database.AddTerminApplication(applicant, termin);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectTerminApplication(input: {{ 
                        terminApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldHaveError(DomainErrors.User.Forbidden);
        var terminApplication = Database.DbContext.Set<TerminApplication>().First(x => x.TerminId == termin.Id);
        terminApplication.Accepted.Should().BeFalse();
        terminApplication.Rejected.Should().BeFalse();
        terminApplication.CompletedOnUtc.Should().BeNull();
        terminApplication.CompletedByUserId.Should().BeNull();
    }

    [Fact]
    public async Task RejectTerminApplication_ShouldFail_WhenApplicationIsAlreadyAccepted()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var termin = Database.AddTermin(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        termin.AddPlayers(new List<Guid>() { user.Id });
        termin.Announce(adminOnEvent.Id, true);
        
        var application = Database.AddTerminApplication(applicant, termin);

        await Database.SaveChangesAsync();
        application.Accept(adminOnEvent, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectTerminApplication(input: {{ 
                        terminApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectTerminApplication", DomainErrors.TerminApplication.AlreadyAccepted);
        var terminApplication = Database.DbContext.Set<TerminApplication>().First(x => x.TerminId == termin.Id);
        terminApplication.Accepted.Should().BeTrue();
        terminApplication.Rejected.Should().BeFalse();
        terminApplication.CompletedOnUtc.Should().Be(completedTime);
        terminApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }

    [Fact]
    public async Task RejectTerminApplication_ShouldFail_WhenApplicationIsAlreadyRejected()
    {
        var completedTime = DateTime.UtcNow;

        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var termin = Database.AddTermin(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        termin.AddPlayers(new List<Guid>() { user.Id });
        termin.Announce(adminOnEvent.Id, true);

        var application = Database.AddTerminApplication(applicant, termin);

        await Database.SaveChangesAsync();
        application.Reject(adminOnEvent, completedTime);
        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectTerminApplication(input: {{ 
                        terminApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeFailureResult("rejectTerminApplication", DomainErrors.TerminApplication.AlreadyRejected);
        var terminApplication = Database.DbContext.Set<TerminApplication>().First(x => x.TerminId == termin.Id);
        terminApplication.Accepted.Should().BeFalse();
        terminApplication.Rejected.Should().BeTrue();
        terminApplication.CompletedOnUtc.Should().Be(completedTime);
        terminApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }

    [Fact]
    public async Task RejectTerminApplication_ShouldSucceed_WhenUserIsAdmin()
    {
        var completedTime = DateTime.UtcNow;
        DateTimeProviderMock.Setup(d => d.UtcNow).Returns(completedTime);
        var user = Database.AddUser();
        var applicant = Database.AddUser("applicant");
        var adminOnEvent = Database.AddUser("admin");
        var termin = Database.AddTermin(adminOnEvent, startDate: DateTime.Today.AddDays(1));
        termin.AddPlayers(new List<Guid>() { user.Id });
        termin.Announce(adminOnEvent.Id, true);

        var application = Database.AddTerminApplication(applicant, termin);

        await Database.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(adminOnEvent.Id);

        var initialApplication = Database.DbContext.Set<TerminApplication>().First(x => x.TerminId == termin.Id);
        initialApplication.Accepted.Should().BeFalse();
        initialApplication.Rejected.Should().BeFalse();
        initialApplication.CompletedOnUtc.Should().BeNull();
        initialApplication.CompletedByUserId.Should().BeNull();

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                    mutation {{
                    rejectTerminApplication(input: {{ 
                        terminApplicationId: ""{application.Id}"" }}){{
                            isSuccess, isFailure, error{{ message, code }}
                        }}}}"));

        result.ShouldBeSuccessResult("rejectTerminApplication");
        var terminApplication = Database.DbContext.Set<TerminApplication>().First(x => x.TerminId == termin.Id);
        terminApplication.Accepted.Should().BeFalse();
        terminApplication.Rejected.Should().BeTrue();
        terminApplication.CompletedOnUtc.Should().Be(completedTime);
        terminApplication.CompletedByUserId.Should().Be(adminOnEvent.Id);
    }
}