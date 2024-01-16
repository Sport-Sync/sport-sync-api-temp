using SportSync.Application.Events.GetDatesByDayOfWeek;

namespace sport_sync.GraphQL.Types.Queries;

[ExtendObjectType("Query")]
public class EventQuery
{
    public async Task<GetDatesByDayOfWeekResponse> GetDatesByDayOfWeek(
        [Service] GetDatesByDayOfWeekInputHandler inputHandler,
        GetDatesByDayOfWeekInput input,
        CancellationToken cancellationToken) => await inputHandler.Handle(input, cancellationToken);

}
