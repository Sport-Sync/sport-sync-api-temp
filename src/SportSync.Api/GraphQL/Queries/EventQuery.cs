using HotChocolate.Authorization;
using SportSync.Application.Events.GetEvents;
using SportSync.Domain.Types;

namespace sport_sync.GraphQL.Queries;

[ExtendObjectType("Query")]
public class EventQuery
{
    [Authorize]
    [UseProjection]
    public async Task<List<EventType>> GetEvents(
        [Service] GetEventsRequestHandler requestHandler,
        CancellationToken cancellationToken) => await requestHandler.Handle(cancellationToken);
}