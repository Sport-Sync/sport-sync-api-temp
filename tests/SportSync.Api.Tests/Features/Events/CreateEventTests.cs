using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Domain.Entities;
using SportSync.Domain.Enumerations;

namespace SportSync.Api.Tests.Features.Events;

[Collection("IntegrationTests")]
public class CreateEventTests : IntegrationTest
{
    [Fact]
    public async Task CreateEvent_WithValidData_ShouldCreateSuccessfully()
    {
        var eventDateTimeTomorrow = DateTime.Today.AddDays(1);
        var eventDateTimeDayAfterTommorow = DateTime.Today.AddDays(2);
        var creator = Database.AddUser();
        var member = Database.AddUser("Marko");
        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(creator.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                            mutation{{
                createEvent(input: {{
                  name: ""Fuca petkom"",
                  memberIds: [""{member.Id}""],
                  address: ""Aleja ruža 12"",
                  numberOfPlayers: 12,
                  notes: """",
                  price: 5,
                  sportType:Football,
                  eventTime:[
                    {{
                        dayOfWeek: {eventDateTimeTomorrow.DayOfWeek},
                        startDate: ""{eventDateTimeTomorrow.ToIsoString()}"",
                        startTime: ""2024-01-20T12:59:39.967Z"",
                        endTime: ""2024-01-20T13:59:39.967Z"",
                        repeatWeekly: false
                    }},
                    {{
                        dayOfWeek: {eventDateTimeDayAfterTommorow.DayOfWeek},
                        startDate: ""{eventDateTimeDayAfterTommorow.ToIsoString()}"",
                        startTime: ""2024-01-22T19:00:00Z"",
                        endTime: ""2024-01-20T20:00:00Z"",
                        repeatWeekly: true
                    }}
                  ] 
                }})
            }}"));

        var eventCreatedId = result.ToResponseObject<Guid>("createEvent");
        eventCreatedId.Should().NotBeEmpty();

        var eventFromDb = Database.DbContext.Set<Event>().FirstOrDefault(x => x.Id == eventCreatedId);
        eventFromDb.Should().NotBeNull();
        eventFromDb.Address.Should().Be("Aleja ruža 12");
        eventFromDb.Name.Should().Be("Fuca petkom");
        eventFromDb.SportType.Should().Be(SportType.Football);

        var termins = Database.DbContext.Set<Termin>().Where(x => x.EventId == eventCreatedId);
        termins.Count().Should().Be(15);
        termins.All(x => x.Status == TerminStatus.Pending).Should().BeTrue();

        var players = Database.DbContext.Set<Player>().Where(x => termins.Select(t => t.Id).Contains(x.TerminId));
        players.Count().Should().Be(30);
        players.Where(x => x.UserId == member.Id).Count().Should().Be(15);
        players.All(p => p.Attending == null).Should().BeTrue();

        var eventMembers = Database.DbContext.Set<EventMember>().Where(x => eventCreatedId == x.EventId);
        eventMembers.Count().Should().Be(2);

        var creatorMember = eventMembers.Single(x => x.UserId == creator.Id);
        creatorMember.IsAdmin.Should().BeTrue();
        creatorMember.IsCreator.Should().BeTrue();

        var memberNonAdmin = eventMembers.Single(x => x.UserId == member.Id);
        memberNonAdmin.IsAdmin.Should().BeFalse();
        memberNonAdmin.IsCreator.Should().BeFalse();
    }

    [Fact]
    public async Task CreateEvent_ShouldFail_WhenStartDateIsTodayButTimePassed()
    {
        var eventDateTimeToday = DateTime.Today;
        var eventDateTimeDayAfterTommorow = DateTime.Today.AddDays(2);
        var creator = Database.AddUser();
        var member = Database.AddUser("Marko");
        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(creator.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                            mutation{{
                createEvent(input: {{
                  name: ""Fuca petkom"",
                  memberIds: [""{member.Id}""],
                  address: ""Aleja ruža 12"",
                  numberOfPlayers: 12,
                  notes: """",
                  price: 5,
                  sportType:Football,
                  eventTime:[
                    {{
                        dayOfWeek: {eventDateTimeToday.DayOfWeek},
                        startDate: ""{eventDateTimeToday.ToIsoString()}"",
                        startTime: ""{DateTime.UtcNow.AddMinutes(-5).ToIsoString()}"",
                        endTime: ""2024-01-20T13:59:39.967Z"",
                        repeatWeekly: false
                    }},
                    {{
                        dayOfWeek: {eventDateTimeDayAfterTommorow.DayOfWeek},
                        startDate: ""{eventDateTimeDayAfterTommorow.ToIsoString()}"",
                        startTime: ""2024-01-22T19:00:00Z"",
                        endTime: ""2024-01-20T20:00:00Z"",
                        repeatWeekly: true
                    }}
                  ] 
                }})
            }}"));

        result.ShouldHaveError("The time for today is invalid.");

        Database.DbContext.Set<Event>().Should().BeEmpty();
        Database.DbContext.Set<Termin>().Should().BeEmpty();
    }

    [Fact]
    public async Task CreateEvent_ShouldFail_WhenStartDateIsHasPassed()
    {
        var eventDateTimeYesterday = DateTime.Today.AddDays(-1);
        var eventDateTimeDayAfterTommorow = DateTime.Today.AddDays(2);
        var creator = Database.AddUser();
        var member = Database.AddUser("Marko");
        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(creator.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                            mutation{{
                createEvent(input: {{
                  name: ""Fuca petkom"",
                  memberIds: [""{member.Id}""],
                  address: ""Aleja ruža 12"",
                  numberOfPlayers: 12,
                  notes: """",
                  price: 5,
                  sportType:Football,
                  eventTime:[
                    {{
                        dayOfWeek: {eventDateTimeYesterday.DayOfWeek},
                        startDate: ""{eventDateTimeYesterday.ToIsoString()}"",
                        startTime: ""2024-01-20T12:59:39.967Z"",
                        endTime: ""2024-01-20T13:59:39.967Z"",
                        repeatWeekly: false
                    }},
                    {{
                        dayOfWeek: {eventDateTimeDayAfterTommorow.DayOfWeek},
                        startDate: ""{eventDateTimeDayAfterTommorow.ToIsoString()}"",
                        startTime: ""2024-01-22T19:00:00Z"",
                        endTime: ""2024-01-20T20:00:00Z"",
                        repeatWeekly: true
                    }}
                  ] 
                }})
            }}"));

        result.ShouldHaveError("StartDate should not be in past.");

        Database.DbContext.Set<Event>().Should().BeEmpty();
        Database.DbContext.Set<Termin>().Should().BeEmpty();
    }

    [Fact]
    public async Task CreateEvent_ShouldFail_WhenStartTimeIsAfterEndTime()
    {
        var eventDateTimeTomorrow = DateTime.Today.AddDays(-1);
        var eventDateTimeDayAfterTommorow = DateTime.Today.AddDays(2);
        var creator = Database.AddUser();
        var member = Database.AddUser("Marko");
        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(creator.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                            mutation{{
                createEvent(input: {{
                  name: ""Fuca petkom"",
                  memberIds: [""{member.Id}""],
                  address: ""Aleja ruža 12"",
                  numberOfPlayers: 12,
                  notes: """",
                  price: 5,
                  sportType:Football,
                  eventTime:[
                    {{
                        dayOfWeek: {eventDateTimeTomorrow.DayOfWeek},
                        startDate: ""{eventDateTimeTomorrow.ToIsoString()}"",
                        startTime: ""2024-01-20T13:59:39.967Z"",
                        endTime: ""2024-01-20T13:55:39.967Z"",
                        repeatWeekly: false
                    }},
                    {{
                        dayOfWeek: {eventDateTimeDayAfterTommorow.DayOfWeek},
                        startDate: ""{eventDateTimeDayAfterTommorow.ToIsoString()}"",
                        startTime: ""2024-01-22T19:00:00Z"",
                        endTime: ""2024-01-20T20:00:00Z"",
                        repeatWeekly: true
                    }}
                  ] 
                }})
            }}"));

        result.ShouldHaveError("StartTime needs to be earlier than EndTime.");

        Database.DbContext.Set<Event>().Should().BeEmpty();
        Database.DbContext.Set<Termin>().Should().BeEmpty();
    }

    [Fact]
    public async Task CreateEvent_ShouldFail_WhenDayOfWeekDoesntMatchStartDate()
    {
        var eventDateTimeTomorrow = DateTime.Today.AddDays(-1);
        var eventDateTimeDayAfterTommorow = DateTime.Today.AddDays(2);
        var creator = Database.AddUser();
        var member = Database.AddUser("Marko");
        await Database.SaveChangesAsync();
        UserIdentifierMock.Setup(x => x.UserId).Returns(creator.Id);

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@$"
                            mutation{{
                createEvent(input: {{
                  name: ""Fuca petkom"",
                  memberIds: [""{member.Id}""],
                  address: ""Aleja ruža 12"",
                  numberOfPlayers: 12,
                  notes: """",
                  price: 5,
                  sportType:Football,
                  eventTime:[
                    {{
                        dayOfWeek: {eventDateTimeTomorrow.AddDays(1).DayOfWeek},
                        startDate: ""{eventDateTimeTomorrow.ToIsoString()}"",
                        startTime: ""2024-01-20T13:59:39.967Z"",
                        endTime: ""2024-01-20T14:55:39.967Z"",
                        repeatWeekly: false
                    }},
                    {{
                        dayOfWeek: {eventDateTimeDayAfterTommorow.DayOfWeek},
                        startDate: ""{eventDateTimeDayAfterTommorow.ToIsoString()}"",
                        startTime: ""2024-01-22T19:00:00Z"",
                        endTime: ""2024-01-20T20:00:00Z"",
                        repeatWeekly: true
                    }}
                  ] 
                }})
            }}"));

        result.ShouldHaveError("StartDate should be on the same day as 'DayOfWeek' input.");

        Database.DbContext.Set<Event>().Should().BeEmpty();
        Database.DbContext.Set<Termin>().Should().BeEmpty();
    }
}