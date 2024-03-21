using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Events.Common;

public class EventInvitationInput : IRequest<Result>
{
    public Guid EventInvitationId { get; set; }
}