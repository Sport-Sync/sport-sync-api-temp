using SportSync.Application.Core.Abstractions.Common;

namespace SportSync.Application.Termins.SetTerminAttendence;

public class SetTerminAttendenceInput : IInput<SetTerminAttendenceResponse>
{
    public Guid TerminId { get; set; }
    public bool Attending { get; set; }
}