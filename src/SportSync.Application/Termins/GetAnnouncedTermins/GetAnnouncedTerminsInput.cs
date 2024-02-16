namespace SportSync.Application.Termins.GetAnnouncedTermins;

public class GetAnnouncedTerminsInput : IRequest<GetAnnouncedTerminResponse>
{
    public DateTime Date { get; set; }
}