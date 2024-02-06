using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Authorization;
using SportSync.Application.Events.CreateEvent;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class EventMutation
{
    [Authorize]
    public async Task<Guid> CreateEvent(
        [Service] CreateEventRequestHandler requestHandler,
        [UseFluentValidation] CreateEventInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}