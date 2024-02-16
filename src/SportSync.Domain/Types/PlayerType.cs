using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class PlayerType
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool? IsAttending { get; set; }

    public static PlayerType FromPlayer(Player player)
    {
        return new PlayerType
        {
            FirstName = player.User.FirstName,
            LastName = player.User.LastName,
            UserId = player.UserId,
            IsAttending = player.Attending
        };
    }
}