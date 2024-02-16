using SportSync.Domain.Types;

namespace SportSync.Application.Termins.GetAnnouncedTermins;

public class GetAnnouncedTerminResponse
{
    public List<TerminType> Termins { get; set; } = new();
}