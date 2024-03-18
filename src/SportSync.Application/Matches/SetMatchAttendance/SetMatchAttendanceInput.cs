namespace SportSync.Application.Matches.SetMatchAttendance;

public class SetMatchAttendanceInput : IRequest<SetMatchAttendanceResponse>
{
    public Guid MatchId { get; set; }
    public bool Attending { get; set; }
}