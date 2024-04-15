using SportSync.Domain.Types;

namespace SportSync.Application.Team.CreateTeam;

public class CreateTeamInput : IRequest<TeamType>
{
    public Guid EventId { get; set; }
    public string Name { get; set; }
}