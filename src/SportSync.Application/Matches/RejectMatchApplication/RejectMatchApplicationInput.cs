using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Matches.RejectMatchApplication;

public class RejectMatchApplicationInput : IRequest<Result>
{
    public Guid MatchApplicationId { get; set; }
}