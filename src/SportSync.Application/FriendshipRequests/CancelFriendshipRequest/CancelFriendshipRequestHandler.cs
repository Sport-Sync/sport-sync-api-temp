using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.FriendshipRequests.Common;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.FriendshipRequests.CancelFriendshipRequest;

public class CancelFriendshipRequestHandler : IRequestHandler<FriendshipRequestInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelFriendshipRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IFriendshipRequestRepository friendshipRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _friendshipRequestRepository = friendshipRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(FriendshipRequestInput request, CancellationToken cancellationToken)
    {
        Maybe<FriendshipRequest> maybeFriendshipRequest = await _friendshipRequestRepository.GetByIdAsync(request.FriendshipRequestId, cancellationToken);

        if (maybeFriendshipRequest.HasNoValue)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.NotFound);
        }

        FriendshipRequest friendshipRequest = maybeFriendshipRequest.Value;

        if (friendshipRequest.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        if (friendshipRequest.Accepted)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.AlreadyAccepted);
        }

        if (friendshipRequest.Rejected)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.AlreadyRejected);
        }

        _friendshipRequestRepository.Remove(friendshipRequest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}