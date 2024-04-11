using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Domain.Types;

namespace SportSync.Application.Core.Services;

public class UserImageService : IUserImageService
{
    private readonly IBlobStorageService _blobStorageService;

    public UserImageService(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    public async Task PopulateImageUrls(params UserType[] users)
    {
        foreach (var user in users.Where(f => f.HasProfileImage))
        {
            user.ImageUrl = await _blobStorageService.GetProfileImageUrl(user.Id);
        }
    }

    public async Task PopulateImageUrls(params FriendshipRequestType[] friendshipRequestTypes)
    {
        foreach (var friendshipRequest in friendshipRequestTypes.Where(f => f.Sender.HasProfileImage))
        {
            friendshipRequest.Sender.ImageUrl = await _blobStorageService.GetProfileImageUrl(friendshipRequest.Sender.Id);
        }
    }
}