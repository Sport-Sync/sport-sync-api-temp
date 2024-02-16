namespace SportSync.Domain.Types;

public class TerminAttendenceType
{
    public bool? IsCurrentUserAttending { get; set; }
    public List<PlayerType> PlayersAttending { get; set; }
    public List<PlayerType> PlayersNotAttending { get; set; }
    public List<PlayerType> PlayersNotResponded { get; set; }
}