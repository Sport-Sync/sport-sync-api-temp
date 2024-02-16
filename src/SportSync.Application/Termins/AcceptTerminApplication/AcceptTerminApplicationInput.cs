using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Termins.AcceptTerminApplication;

public class AcceptTerminApplicationInput : IRequest<Result>
{
    public Guid TerminApplicationId { get; set; }
}