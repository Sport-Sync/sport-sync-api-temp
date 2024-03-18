using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Matches.AcceptMatchApplication;

public class AcceptMatchApplicationInput : IRequest<Result>
{
    public Guid MatchApplicationId { get; set; }
}