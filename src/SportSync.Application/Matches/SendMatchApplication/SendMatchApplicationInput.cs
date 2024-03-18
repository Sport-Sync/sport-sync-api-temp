using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Matches.SendMatchApplication;

public class SendMatchApplicationInput : IRequest<Result>
{
    public Guid MatchId { get; set; }
}