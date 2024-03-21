using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Matches.Common;

public class MatchApplicationInput : IRequest<Result>
{
    public Guid MatchApplicationId { get; set; }
}