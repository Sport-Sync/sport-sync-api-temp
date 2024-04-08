namespace SportSync.Application.Matches.GetMatches;
using MatchType = Domain.Types.MatchType;

public class GetMatchesResponse
{
    public List<MatchType> Matches { get; set; }
}