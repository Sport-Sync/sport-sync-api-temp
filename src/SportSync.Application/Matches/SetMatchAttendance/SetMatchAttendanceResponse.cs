using SportSync.Domain.Types;

namespace SportSync.Application.Matches.SetMatchAttendance;

public class SetMatchAttendanceResponse
{
    public List<PlayerType> Players { get; set; }
}