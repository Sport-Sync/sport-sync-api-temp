namespace SportSync.Application.Termins.SetTerminAttendence;

public class SetTerminAttendenceInput : IRequest<SetTerminAttendenceResponse>
{
    public Guid TerminId { get; set; }
    public bool Attending { get; set; }
}