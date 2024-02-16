using SportSync.Domain.Types;

namespace SportSync.Application.Termins.GetTerminById;

public class GetTerminByIdResponse
{
    public TerminType Termin { get; set; }

    public TerminAttendenceType Attendence { get; set; }
}