namespace SportSync.Application.Matches.GetAnnouncedMatches;

public class GetAnnouncedMatchesInput : IRequest<GetAnnouncedMatchesResponse>
{
    public DateTime Date { get; set; }
}