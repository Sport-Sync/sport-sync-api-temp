using Microsoft.Extensions.Logging;
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
    private readonly ILogger<SendFriendshipRequestHandler> _logger;

    public SendFriendshipRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IFriendshipRequestRepository friendshipRequestRepository,
        IUnitOfWork unitOfWork, 
        ILogger<SendFriendshipRequestHandler> logger)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _friendshipRequestRepository = friendshipRequestRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SendFriendshipRequestInput request, CancellationToken cancellationToken)
    {
        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        var friends = await _userRepository.GetByIdsAsync(request.FriendIds, cancellationToken);

        if (!friends.Any())
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var user = maybeUser.Value;

        var friendshipRequests = new List<Result<FriendshipRequest>>();
        var existingFriendshipRequests = await _friendshipRequestRepository.GetAllPendingForUserIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        foreach (var friend in friends)
        {
            if (existingFriendshipRequests.Any(x => x.FriendId == friend.Id || x.UserId == friend.Id))
            {
                return Result.Failure<FriendshipRequest>(DomainErrors.FriendshipRequest.PendingFriendshipRequest);
            }

            var friendshipRequestResult = user.SendFriendshipRequest(friend);

            if (friendshipRequestResult.IsFailure)
            {
                _logger.LogError("Failed to send friendship request from user {userId} to friend {friendId}. Reason: {error}",
                    user.Id, friend.Id, friendshipRequestResult.Error.Message);
            }

            friendshipRequests.Add(friendshipRequestResult);
        }

        if (friendshipRequests.Any(x => x.IsFailure))
        {
            var countFailed = friendshipRequests.Count(x => x.IsFailure);

            _logger.LogError("Failed to send {countFailed}/{totalCount} friendship request(s).",
                countFailed, friendshipRequests.Count);
        }

        if (friendshipRequests.All(x => x.IsFailure))
        {
            return Result.Failure(friendshipRequests.First().Error);
        }

        var validRequests = friendshipRequests.Where(r => r.IsSuccess)
            .Select(r => r.Value)
            .ToList();

        _friendshipRequestRepository.InsertRange(validRequests);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}