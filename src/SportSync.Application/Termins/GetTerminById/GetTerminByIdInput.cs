namespace SportSync.Application.Termins.GetTerminById;

public class GetTerminByIdInput : IRequest<GetTerminByIdResponse>
{
    public Guid TerminId { get; set; }
}