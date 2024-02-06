using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.FriendshipRequests.SendFriendshipRequest;

public class SendFriendshipRequestHandler : IRequestHandler<SendFriendshipRequestInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;

    public SendFriendshipRequestHandler(IUserIdentifierProvider userIdentifierProvider, IUserRepository userRepository)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(SendFriendshipRequestInput request, CancellationToken cancellationToken)
    {
        if (request.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(request.UserId);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        Maybe<User> maybeFriend = await _userRepository.GetByIdAsync(request.FriendId);

        if (maybeFriend.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        return Result.Success();
    }
}