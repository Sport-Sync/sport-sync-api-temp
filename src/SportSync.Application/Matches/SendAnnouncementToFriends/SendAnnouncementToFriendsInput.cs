using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Matches.SendAnnouncementToFriends;

public class SendAnnouncementToFriendsInput : IRequest<Result>
{
    public Guid MatchId { get; set; }
}