using SportSync.Domain.Entities;

namespace SportSync.Domain.Types;

public class MatchApplicationType
{
    public Guid MatchId { get; set; }
    public Guid MatchApplicationId { get; set; }
    public string MatchName { get; set; }
    public bool IsCurrentUserAdmin { get; set; }

    public MatchApplicationType(Match match, MatchApplication matchApplication, bool isCurrentUserAdmin)
    {
        MatchId = match.Id;
        MatchApplicationId = matchApplication.Id;
        MatchName = match.EventName;
        IsCurrentUserAdmin = isCurrentUserAdmin;
    }

    public MatchApplicationType()
    {
        
    }
}