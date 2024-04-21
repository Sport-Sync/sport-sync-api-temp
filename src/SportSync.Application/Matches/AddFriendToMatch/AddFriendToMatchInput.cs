using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Matches.AddFriendToMatch;

public class AddFriendToMatchInput : IRequest<Result>
{
    public Guid FriendId { get; set; }
    public Guid MatchId { get; set; }
}