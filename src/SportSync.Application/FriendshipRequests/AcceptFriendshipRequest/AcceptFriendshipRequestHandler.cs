using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.FriendshipRequests.AcceptFriendshipRequest;

public class AcceptFriendshipRequestHandler : IRequestHandler<AcceptFriendshipRequestInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public AcceptFriendshipRequestHandler(
        IUserIdentifierProvider userIdentifierProvider, 
        IFriendshipRequestRepository friendshipRequestRepository, 
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork, 
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _friendshipRequestRepository = friendshipRequestRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(AcceptFriendshipRequestInput request, CancellationToken cancellationToken)
    {
        Maybe<FriendshipRequest> maybeFriendshipRequest = await _friendshipRequestRepository.GetByIdAsync(request.FriendshipRequestId);

        if (maybeFriendshipRequest.HasNoValue)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.NotFound);
        }

        FriendshipRequest friendshipRequest = maybeFriendshipRequest.Value;

        if (friendshipRequest.FriendId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(friendshipRequest.UserId);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        Maybe<User> maybeFriend = await _userRepository.GetByIdAsync(friendshipRequest.FriendId);

        if (maybeFriend.HasNoValue)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.FriendNotFound);
        }

        Result acceptResult = friendshipRequest.Accept(_dateTime.UtcNow);

        if (acceptResult.IsFailure)
        {
            return Result.Failure(acceptResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}