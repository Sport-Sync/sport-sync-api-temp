using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Core.Constants;

namespace SportSync.Application.Users.GetProfileImageUrl;

public class ProfileImageUrlRequestHandler : IRequestHandler<ProfileImageUrlInput, ProfileImageUrlResponse>
{
    private readonly IBlobStorageService _blobStorageService;

    public ProfileImageUrlRequestHandler(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    public async Task<ProfileImageUrlResponse> Handle(ProfileImageUrlInput request, CancellationToken cancellationToken)
    {
        var fileName = string.Format(StringFormatConstants.ProfileImageFilePathFormat, request.UserId);

        var url = await _blobStorageService.GetDownloadUrl(fileName);

        return new ProfileImageUrlResponse { ImageUrl = url };
    }
}