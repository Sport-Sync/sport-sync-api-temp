using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Core.Constants;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Users.GetProfileImageUrl;

public class ProfileImageUrlRequestHandler : IRequestHandler<ProfileImageUrlInput, ProfileImageUrlResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorageService _blobStorageService;

    public ProfileImageUrlRequestHandler(IBlobStorageService blobStorageService, IUserRepository userRepository)
    {
        _blobStorageService = blobStorageService;
        _userRepository = userRepository;
    }

    public async Task<ProfileImageUrlResponse> Handle(ProfileImageUrlInput request, CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        var user = maybeUser.Value;

        if (!user.HasProfileImage)
        {
            return new ProfileImageUrlResponse();
        }

        var fileName = string.Format(StringFormatConstants.ProfileImageFilePathFormat, request.UserId);

        var url = await _blobStorageService.GetDownloadUrl(fileName);

        return new ProfileImageUrlResponse { ImageUrl = url };
    }
}