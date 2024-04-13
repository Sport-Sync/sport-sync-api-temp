using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Matches.SetMatchAttendance;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Matches;

[Collection("IntegrationTests")]
public class SetMatchAttendanceTests : IntegrationTest
{
    [Fact]
    public async Task SetAttendance_ShouldFail_WhenMatchNotFound()
    {
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setMatchAttendance(input: {{matchId: ""{Guid.NewGuid()}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Match.NotFound);
    }

    [Fact]
    public async Task SetAttendance_ShouldFail_WhenUserIsNotAPlayer()
    {
        var user = Database.AddUser();
        var user2 = Database.AddUser("second", "user", "user@gmail.com");
        var match = Database.AddMatch(user2, startDate: DateTime.Today.AddDays(1));

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setMatchAttendance(input: {{matchId: ""{match.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        result.ShouldHaveError(DomainErrors.Match.PlayerNotFound);
    }

    [Fact]
    public async Task SetAttendance_ShouldUpdateAttendance()
    {
        var user = Database.AddUser();
        var match = Database.AddMatch(user, startDate: DateTime.Today.AddDays(1));

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        // Set to true
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setMatchAttendance(input: {{matchId: ""{match.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        var response = result.ToResponseObject<SetMatchAttendanceResponse>("setMatchAttendance");

        response.Players.Count.Should().Be(1);

        response.Players.Single().UserId.Should().Be(user.Id);
        response.Players.Single().FirstName.Should().Be(user.FirstName);
        response.Players.Single().IsAttending.Should().BeTrue();

        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.MatchId == match.Id).Attending.Should().BeTrue();

        // Set back to false
        var secondResult = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setMatchAttendance(input: {{matchId: ""{match.Id}"", attending: false}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        var secondResponse = secondResult.ToResponseObject<SetMatchAttendanceResponse>("setMatchAttendance");

        secondResponse.Players.Count.Should().Be(1);

        secondResponse.Players.Single().UserId.Should().Be(user.Id);
        secondResponse.Players.Single().FirstName.Should().Be(user.FirstName);
        secondResponse.Players.Single().IsAttending.Should().BeFalse();

        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.MatchId == match.Id).Attending.Should().BeFalse();
    }

    [Fact]
    public async Task SetAttendance_ShouldUpdateAttendance_WhenMatchIsLaterToday()
    {
        var user = Database.AddUser();
        var schedule = EventSchedule.Create(
            DayOfWeek.Wednesday, new DateTime(2024, 1, 1), DateTime.UtcNow.AddMinutes(5), DateTime.MaxValue, true);
        var match = Database.AddMatch(user, startDate: DateTime.Today, schedule: schedule);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        // Set to true
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setMatchAttendance(input: {{matchId: ""{match.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));


        var response = result.ToResponseObject<SetMatchAttendanceResponse>("setMatchAttendance");

        response.Players.Count.Should().Be(1);

        response.Players.Single().UserId.Should().Be(user.Id);
        response.Players.Single().FirstName.Should().Be(user.FirstName);
        response.Players.Single().IsAttending.Should().BeTrue();

        Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.MatchId == match.Id).Attending.Should().BeTrue();
    }

    [Theory]
    [InlineData(MatchStatusEnum.Finished, false)]
    [InlineData(MatchStatusEnum.Canceled, false)]
    [InlineData(MatchStatusEnum.Pending, true)]
    [InlineData(MatchStatusEnum.InProgress, false)]
    public async Task SetAttendance_ShouldSucceed_OnlyWhenIsPendingStatus(MatchStatusEnum status, bool shouldSucceed)
    {
        var user = Database.AddUser();
        var schedule = EventSchedule.Create(
            DayOfWeek.Wednesday, new DateTime(2024, 1, 1), DateTime.UtcNow.AddHours(2), DateTime.MaxValue, true);
        var match = Database.AddMatch(user, startDate: DateTime.Today.AddDays(-1), schedule: schedule, status: status);

        await Database.UnitOfWork.SaveChangesAsync();

        UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        // Set to true
        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                mutation {{
                    setMatchAttendance(input: {{matchId: ""{match.Id}"", attending: true}}){{
                        players{{
                            firstName, isAttending, userId
                        }}
                    }}
                }}"));

        if (shouldSucceed)
        {
            var response = result.ToResponseObject<SetMatchAttendanceResponse>("setMatchAttendance");

            response.Players.Count.Should().Be(1);

            response.Players.Single().UserId.Should().Be(user.Id);
            response.Players.Single().FirstName.Should().Be(user.FirstName);
            response.Players.Single().IsAttending.Should().BeTrue();

            Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.MatchId == match.Id).Attending.Should().BeTrue();
        }
        else
        {
            result.ShouldHaveError(status.ToError());
            Database.DbContext.Set<Player>().FirstOrDefault(x => x.UserId == user.Id && x.MatchId == match.Id).Attending.Should().BeNull();
        }
    }
}