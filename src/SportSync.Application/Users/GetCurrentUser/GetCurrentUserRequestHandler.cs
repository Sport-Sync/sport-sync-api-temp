using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetCurrentUser;

public class GetCurrentUserRequestHandler : IRequestHandler<UserType>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetCurrentUserRequestHandler(IUserIdentifierProvider userIdentifierProvider, IUserRepository userRepository, IBlobStorageService blobStorageService)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<UserType> Handle(CancellationToken cancellationToken)
    {
        var user = _userRepository.GetQueryable(x => x.Id == _userIdentifierProvider.UserId).SingleOrDefault();

        if (user is null)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        if (user.HasProfileImage)
        {
            user.ImageUrl = await _blobStorageService.GetProfileImageUrl(user.Id);
        }

        return user;
    }
}