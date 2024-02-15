using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Termins.SendTerminApplication;

public class SendTerminApplicationInput : IRequest<Result>
{
    public Guid TerminId { get; set; }
}