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
    [InlineData(TerminStatus.AnnouncedInternally, true)]
    [InlineData(TerminStatus.AnnouncedPublicly, true)]
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
            var terminResponse = result.ToObject<TerminType>("announceTermin");
            terminResponse.Status.Should().Be(TerminStatus.AnnouncedPublicly);
            Database.DbContext.Set<Termin>().Find(termin.Id).Status.Should().Be(TerminStatus.AnnouncedPublicly);
        }
        else
        {
            result.ShouldHaveError(DomainErrors.Termin.AlreadyFinished);
            Database.DbContext.Set<Termin>().Find(termin.Id).Status.Should().Be(status);
        }
    }

    [Theory]
    [InlineData(true, TerminStatus.AnnouncedPublicly)]
    [InlineData(false, TerminStatus.AnnouncedInternally)]
    public async Task Announce_ShouldSucceed_AndSetProperStatus(bool publicly, TerminStatus expectedStatus)
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


        var terminResponse = result.ToObject<TerminType>("announceTermin");
        terminResponse.Status.Should().Be(expectedStatus);
        Database.DbContext.Set<Termin>().Find(termin.Id).Status.Should().Be(expectedStatus);

    }
}