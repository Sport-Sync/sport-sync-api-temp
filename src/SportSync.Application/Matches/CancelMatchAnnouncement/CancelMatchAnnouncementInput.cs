using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Matches.CancelMatchAnnouncement;

public class CancelMatchAnnouncementInput : IRequest<Result>
{
    public Guid MatchId { get; set; }
}