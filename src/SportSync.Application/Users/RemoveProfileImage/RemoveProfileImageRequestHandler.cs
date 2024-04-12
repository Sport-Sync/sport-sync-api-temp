using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Core.Constants;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Users.RemoveProfileImage;

public class RemoveProfileImageRequestHandler : IRequestHandler<Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProfileImageRequestHandler(
        IUserRepository userRepository, 
        IUserIdentifierProvider userIdentifierProvider, 
        IBlobStorageService blobStorageService, 
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _blobStorageService = blobStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var user = maybeUser.Value;

        if (string.IsNullOrEmpty(user.ImageUrl))
        {
            return Result.Success();
        }

        var fileName = string.Format(StringFormatConstants.ProfileImageFilePathFormat, userId);

        var result = await _blobStorageService.RemoveFile(fileName, cancellationToken);

        if (result.IsSuccess)
        {
            user.ImageUrl = null;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;

    }
}