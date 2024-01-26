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
                  sportType:FOOTBALL,
                  eventTime:[
                    {{
                        dayOfWeek: {eventDateTimeTomorrow.DayOfWeek.ToString().ToUpper()},
                        startDate: ""{eventDateTimeTomorrow}"",
                        startTime: ""2024-01-20 12:59:39.967"",
                        endTime: ""2024-01-20 13:59:39.967"",
                        repeatWeekly: false
                    }},
                    {{
                        dayOfWeek: {eventDateTimeDayAfterTommorow.DayOfWeek.ToString().ToUpper()},
                        startDate: ""{eventDateTimeDayAfterTommorow}"",
                        startTime: ""2024-01-22 19:00:00"",
                        endTime: ""2024-01-20 20:00:00"",
                        repeatWeekly: true
                    }}
                  ] 
                }})
            }}"));

        var eventCreatedId = result.ToObject<Guid>("createEvent");
        eventCreatedId.Should().NotBeEmpty();

        var eventFromDb = Database.DbContext.Set<Event>().FirstOrDefault(x => x.Id == eventCreatedId);
        eventFromDb.Should().NotBeNull();
        eventFromDb.Address.Should().Be("Aleja ruža 12");
        eventFromDb.Name.Should().Be("Fuca petkom");
        eventFromDb.SportType.Should().Be(SportType.Football);

        var termins = Database.DbContext.Set<Termin>().Where(x => x.EventId == eventCreatedId);
        termins.Count().Should().Be(15);
        termins.All(x => x.Status == TerminStatus.Open).Should().BeTrue();

        var players = Database.DbContext.Set<Player>().Where(x => termins.Select(t => t.Id).Contains(x.TerminId));
        players.Count().Should().Be(30);
        players.Where(x => x.UserId == member.Id).Count().Should().Be(15);


        var eventMembers = Database.DbContext.Set<EventMember>().Where(x => eventCreatedId == x.EventId);
        eventMembers.Count().Should().Be(2);

        var creatorMember = eventMembers.Single(x => x.UserId == creator.Id);
        creatorMember.IsAdmin.Should().BeTrue();
        creatorMember.IsCreator.Should().BeTrue();

        var memberNonAdmin = eventMembers.Single(x => x.UserId == member.Id);
        memberNonAdmin.IsAdmin.Should().BeFalse();
        memberNonAdmin.IsCreator.Should().BeFalse();
    }
}