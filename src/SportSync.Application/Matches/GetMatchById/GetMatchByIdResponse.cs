using SportSync.Domain.Types;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.GetMatchById;

public class GetMatchByIdResponse
{
    public bool IsCurrentUserAdmin { get; set; }
    public MatchType Match { get; set; }
    public MatchAttendanceType Attendance { get; set; }
    public List<UserType> PendingApplicants { get; set; }
}