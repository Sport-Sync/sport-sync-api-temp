using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Events.SendEventInvitation;

public class SendEventInvitationInput : IRequest<Result>
{
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
}