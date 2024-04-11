namespace SportSync.Application.Users.GetUserProfile;

public class GetUserProfileInput : IRequest<UserProfileResponse>
{
    public Guid UserId { get; set; }
}