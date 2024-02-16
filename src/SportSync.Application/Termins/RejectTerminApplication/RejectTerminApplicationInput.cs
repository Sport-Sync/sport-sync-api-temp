using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Termins.AcceptTerminApplication;

public class RejectTerminApplicationInput : IRequest<Result>
{
    public Guid TerminApplicationId { get; set; }
}