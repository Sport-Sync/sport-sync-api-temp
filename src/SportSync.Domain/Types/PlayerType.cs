using HotChocolate;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class PlayerType
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ImageUrl { get; set; }
    public bool? IsAttending { get; set; }
    [GraphQLIgnore]
    public bool HasProfileImage { get; set; }

    public static PlayerType FromPlayer(Player player)
    {
        return new PlayerType
        {
            FirstName = player.User.FirstName,
            LastName = player.User.LastName,
            UserId = player.UserId,
            IsAttending = player.Attending,
            HasProfileImage = player.User.HasProfileImage
        };
    }
}