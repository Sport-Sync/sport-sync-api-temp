namespace SportSync.Application.Matches.GetAnnouncedMatches;

public class GetAnnouncedMatchesInput : IRequest<GetAnnouncedMatchResponse>
{
    public DateTime Date { get; set; }
}