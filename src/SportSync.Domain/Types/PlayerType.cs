namespace SportSync.Domain.Types;

public class PlayerType
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool? IsAttending { get; set; }
}