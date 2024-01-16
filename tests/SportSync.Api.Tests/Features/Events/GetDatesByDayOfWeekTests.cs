using FluentAssertions;
using SportSync.Api.Tests.Common;
using SportSync.Api.Tests.Extensions;
using SportSync.Application.Events.GetDatesByDayOfWeek;

namespace SportSync.Api.Tests.Features.Events;

[Collection("IntegrationTests")]
public class GetDatesByDayOfWeekTests : IntegrationTest
{
    [Fact]
    public async Task GetDatesByDayOfWeek_ShouldReturnCorrectDates()
    {
        DateTimeProviderMock.Setup(x => x.UtcNow).Returns(new DateTime(2024, 1, 18));

        var result = await ExecuteRequestAsync(
            q => q.SetQuery(@"
            query{
                datesByDayOfWeek(input: {
                    dayOfWeek: WEDNESDAY,
                    numberOfDates: 2
                }){
                    dates
                }
            }"));

        var response = result.ToObject<GetDatesByDayOfWeekResponse>("datesByDayOfWeek");

        response.Dates.Count.Should().Be(2);
        response.Dates.All(x => x.DayOfWeek == DayOfWeek.Wednesday).Should().BeTrue();
        response.Dates.Should().Contain(x => x.Day == 24);
        response.Dates.Should().Contain(x => x.Day == 31);
    }
}