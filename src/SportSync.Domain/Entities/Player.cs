using SportSync.Domain.Core.Primitives;

namespace SportSync.Domain.Entities;

public class Player : Entity
{
    private Player(Guid userId, Guid terminId)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        TerminId = terminId;
    }

    private Player()
    {
    }

    public Guid TerminId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool? Attending { get; set; }

    public static Player Create(Guid userId, Guid terminId)
    {
        return new Player(userId, terminId);
    }
}