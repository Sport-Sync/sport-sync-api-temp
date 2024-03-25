using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUserProfile;

public class GetUserProfileInput : IRequest<UserProfileType>
{
    public Guid UserId { get; set; }
}