using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Core.Constants;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Users.UploadProfileImage;

public class UploadProfileImageRequestHandler : IRequestHandler<UploadProfileImageInput, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UploadProfileImageRequestHandler(
        IBlobStorageService blobStorageService,
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _blobStorageService = blobStorageService;
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UploadProfileImageInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var user = maybeUser.Value;
        var fileName = string.Format(StringFormatConstants.ProfileImageFilePathFormat, userId);

        var result = await _blobStorageService.UploadFile(fileName, request.File, cancellationToken);

        if (result.IsSuccess)
        {
            user.ImageUrl = result.Value;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}