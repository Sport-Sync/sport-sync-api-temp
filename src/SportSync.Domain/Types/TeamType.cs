using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class TeamType
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Name { get; set; }

    public static TeamType FromTeam(Team team) => new()
    {
        Id = team.Id,
        Name = team.Name,
        EventId = team.EventId
    };
}