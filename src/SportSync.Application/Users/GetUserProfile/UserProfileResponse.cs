using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUserProfile;

public class UserProfileResponse
{
    public ExtendedUserType User { get; set; } = new ();
    public List<MatchApplicationType> MatchApplications { get; set; } = new();
}