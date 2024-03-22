namespace SportSync.Application.Users.GetProfileImageUrl;

public class ProfileImageUrlInput : IRequest<ProfileImageUrlResponse>
{
    public Guid UserId { get; set; }
}