using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Termins.SetTerminAttendence;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;

namespace SportSync.Api.Tests.Features.Termins;

[Collection("IntegrationTests")]
public class SetTerminAttendenceTests : IntegrationTest
{
    [Fact]
    public async Task CreateAttendence_ShouldFail_WhenTerminNotFound()
    {
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{Guid.NewGuid()}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Termin.NotFound);
    }

    [Fact]
    public async Task CreateAttendence_ShouldFail_WhenUserIsNotAPlayer()
    {
        var user = Database.AddUser();
        var user2 = Database.AddUser("second", "user", "user@gmail.com", "034234329");
        var termin = Database.AddTermin(user2, startDate: DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{termin.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Termin.PlayerNotFound);
    }

    [Fact]
    public async Task CreateAttendence_ShouldUpdateAttendence()
    {
        var user = Database.AddUser();
        var termin = Database.AddTermin(user, startDate: DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        // Set to true
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{termin.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        var response = result.ToObject<SetTerminAttendenceResponse>("setTerminAttendence");

        response.Players.Count.Should().Be(1);

        response.Players.Single().UserId.Should().Be(user.Id);
        response.Players.Single().FirstName.Should().Be(user.FirstName);
        response.Players.Single().IsAttending.Should().BeTrue();

        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.TerminId == termin.Id).Attending.Should().BeTrue();

        // Set back to false
        var secondResult = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{termin.Id}"", attending: false}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        var secondResponse = secondResult.ToObject<SetTerminAttendenceResponse>("setTerminAttendence");

        secondResponse.Players.Count.Should().Be(1);

        secondResponse.Players.Single().UserId.Should().Be(user.Id);
        secondResponse.Players.Single().FirstName.Should().Be(user.FirstName);
        secondResponse.Players.Single().IsAttending.Should().BeFalse();

        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.TerminId == termin.Id).Attending.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAttendence_ShouldUpdateAttendence_WhenTerminIsLaterToday()
    {
        var user = Database.AddUser();
        var schedule = EventSchedule.Create(
            DayOfWeek.Wednesday, new DateOnly(2024, 1, 1), TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(1)), TimeOnly.MaxValue, true);
        var termin = Database.AddTermin(user, startDate: DateOnly.FromDateTime(DateTime.Today), schedule: schedule);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        // Set to true
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{termin.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));


        var response = result.ToObject<SetTerminAttendenceResponse>("setTerminAttendence");

        response.Players.Count.Should().Be(1);

        response.Players.Single().UserId.Should().Be(user.Id);
        response.Players.Single().FirstName.Should().Be(user.FirstName);
        response.Players.Single().IsAttending.Should().BeTrue();

        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.TerminId == termin.Id).Attending.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAttendence_ShouldFail_WhenTerminStartDatePassed()
    {
        var user = Database.AddUser();
        var schedule = EventSchedule.Create(
            DayOfWeek.Wednesday, new DateOnly(2024, 1, 1), TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(2)), TimeOnly.MaxValue, true);
        var termin = Database.AddTermin(user, startDate: DateOnly.FromDateTime(DateTime.Today).AddDays(-1), schedule: schedule);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        // Set to true
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{termin.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));
        
        result.ShouldHaveError(DomainErrors.Termin.AlreadyFinished);
        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.TerminId == termin.Id).Attending.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAttendence_ShouldFail_WhenTerminStartTimePassed()
    {
        var user = Database.AddUser();
        var schedule = EventSchedule.Create(
            DayOfWeek.Wednesday, new DateOnly(2024, 1, 1), TimeOnly.FromDateTime(DateTime.UtcNow.AddMinutes(-20)), TimeOnly.MaxValue, true);
        var termin = Database.AddTermin(user, startDate: DateOnly.FromDateTime(DateTime.Today), schedule: schedule);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        // Set to true
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setTerminAttendence(input: {{terminId: ""{termin.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Termin.AlreadyFinished);
        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.TerminId == termin.Id).Attending.Should().BeFalse();
    }
}