using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Authorization;
using SportSync.Application.Events.CreateEvent;

namespace sport_sync.GraphQL.Types.Mutations;

[ExtendObjectType("Mutation")]
public class EventMutation
{
    [Authorize]
    public async Task<Guid> CreateEvent(
        [Service] CreateEventInputHandler inputHandler,
        [UseFluentValidation] CreateEventInput input,
        CancellationToken cancellationToken) => await inputHandler.Handle(input, cancellationToken);
}