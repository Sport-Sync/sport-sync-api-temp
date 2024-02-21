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
        if (request.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var friends = await _userRepository.GetByIdsAsync(request.FriendIds, cancellationToken);

        if (!friends.Any())
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var user = maybeUser.Value;

        var friendshipRequests = new List<Result<FriendshipRequest>>();
        foreach (var friend in friends)
        {
            var friendshipRequestResult = await user.SendFriendshipRequest(_friendshipRequestRepository, friend);

            if (friendshipRequestResult.IsFailure)
            {
                _logger.LogError("Failed to send friendship request from user {userId} to friend {friendId}. Reason: {error}", 
                    user.Id, 
                    friend.Id, 
                    friendshipRequestResult.Error.Message);

                continue;
            }

            friendshipRequests.Add(friendshipRequestResult);
        }

        if (friendshipRequests.All(x => x.IsFailure))
        {
            return Result.Failure(friendshipRequests.First().Error);
        }

        _friendshipRequestRepository.InsertRange(friendshipRequests.Select(f => f.Value).ToList());

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}