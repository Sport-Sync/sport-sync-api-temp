using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
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
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendFriendshipRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IFriendshipRequestRepository friendshipRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _friendshipRequestRepository = friendshipRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendFriendshipRequestInput request, CancellationToken cancellationToken)
    {
        if (request.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        Maybe<User> maybeFriend = await _userRepository.GetByIdAsync(request.FriendId, cancellationToken);

        if (maybeFriend.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var user = maybeUser.Value;

        var friendshipRequestResult = await user.SendFriendshipRequest(_friendshipRequestRepository, maybeFriend.Value);

        if (friendshipRequestResult.IsFailure)
        {
            return Result.Failure(friendshipRequestResult.Error);
        }
        
        _friendshipRequestRepository.Insert(friendshipRequestResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}