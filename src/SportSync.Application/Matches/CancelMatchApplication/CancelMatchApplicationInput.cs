using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Matches.CancelMatchApplication;

public class CancelMatchApplicationInput : IRequest<Result>
{
    public Guid MatchApplicationId { get; set; }
}