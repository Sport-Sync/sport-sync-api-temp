using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Core.Services;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetCurrentUser;

public class GetCurrentUserRequestHandler : IRequestHandler<UserType>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUserImageService _userImageService;

    public GetCurrentUserRequestHandler(IUserIdentifierProvider userIdentifierProvider, IUserRepository userRepository, IUserImageService userImageService)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _userImageService = userImageService;
    }

    public async Task<UserType> Handle(CancellationToken cancellationToken)
    {
        var user = _userRepository.GetQueryable(x => x.Id == _userIdentifierProvider.UserId).SingleOrDefault();

        if (user is null)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        await _userImageService.PopulateImageUrl(user);

        return user;
    }
}