using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Core.Constants;
using SportSync.Domain.Core.Primitives.Result;

namespace SportSync.Application.Users.UploadProfileImage;

public class UploadProfileImageRequestHandler : IRequestHandler<UploadProfileImageInput, Result>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public UploadProfileImageRequestHandler(IBlobStorageService blobStorageService, IUserIdentifierProvider userIdentifierProvider)
    {
        _blobStorageService = blobStorageService;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Result> Handle(UploadProfileImageInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;
        var fileName = string.Format(StringFormatConstants.ProfileImageFilePathFormat, userId);

        return await _blobStorageService.UploadFile(fileName, request.File, cancellationToken);
    }
}