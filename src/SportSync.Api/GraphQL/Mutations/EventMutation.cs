using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Authorization;
using SportSync.Application.Events.CreateEvent;
using SportSync.Application.Events.SendEventInvitation;
using SportSync.Domain.Core.Primitives.Result;

namespace sport_sync.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class EventMutation
{
    [Authorize]
    public async Task<Guid> CreateEvent(
        [Service] CreateEventRequestHandler requestHandler,
        [UseFluentValidation] CreateEventInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);

    [Authorize]
    public async Task<Result> SendEventInvitation(
        [Service] SendEventInvitationRequestHandler requestHandler,
        SendEventInvitationInput input,
        CancellationToken cancellationToken) => await requestHandler.Handle(input, cancellationToken);
}