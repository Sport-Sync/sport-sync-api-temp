using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.GetAnnouncedMatches;

public class GetAnnouncedMatchResponse
{
    public List<MatchType> Matches { get; set; } = new();
}