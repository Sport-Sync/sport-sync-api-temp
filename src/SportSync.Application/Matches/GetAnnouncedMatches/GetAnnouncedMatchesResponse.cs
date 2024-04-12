using SportSync.Domain.Types;

namespace SportSync.Application.Matches.GetAnnouncedMatches;

public class GetAnnouncedMatchesResponse
{
    public List<MatchAnnouncementType> Matches { get; set; } = new();
}