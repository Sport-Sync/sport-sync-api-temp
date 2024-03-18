namespace SportSync.Application.Matches.GetMatchById;

public class GetMatchByIdInput : IRequest<GetMatchByIdResponse>
{
    public Guid MatchId { get; set; }
}