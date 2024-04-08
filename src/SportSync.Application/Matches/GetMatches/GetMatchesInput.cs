namespace SportSync.Application.Matches.GetMatches;

public class GetMatchesInput : IRequest<GetMatchesResponse>
{
    public DateTime Date { get; set; }
}