using SportSync.Domain.Types;

namespace SportSync.Application.Matches.GetAnnouncedMatches;

public class GetAnnouncedMatchResponse
{
    public List<MatchAnnouncementType> Matches { get; set; } = new();
}