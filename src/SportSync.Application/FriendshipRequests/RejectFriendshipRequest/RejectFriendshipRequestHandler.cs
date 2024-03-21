using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.FriendshipRequests.Common;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.FriendshipRequests.RejectFriendshipRequest;

public class RejectFriendshipRequestHandler : IRequestHandler<FriendshipRequestInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public RejectFriendshipRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IFriendshipRequestRepository friendshipRequestRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _friendshipRequestRepository = friendshipRequestRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(FriendshipRequestInput request, CancellationToken cancellationToken)
    {
        Maybe<FriendshipRequest> maybeFriendshipRequest = await _friendshipRequestRepository.GetByIdAsync(request.FriendshipRequestId, cancellationToken);

        if (maybeFriendshipRequest.HasNoValue)
        {
            return Result.Failure(DomainErrors.FriendshipRequest.NotFound);
        }

        FriendshipRequest friendshipRequest = maybeFriendshipRequest.Value;

        if (friendshipRequest.FriendId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        Result rejectResult = friendshipRequest.Reject(_dateTime.UtcNow);

        if (rejectResult.IsFailure)
        {
            return Result.Failure(rejectResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}