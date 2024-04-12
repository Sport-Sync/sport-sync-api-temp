using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Services;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Services;
using SportSync.Domain.Types;

namespace SportSync.Application.Users.GetUserProfile;

public class GetUserProfileRequestHandler : IRequestHandler<GetUserProfileInput, UserProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IFriendshipRequestRepository _friendshipRequestRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchApplicationRepository _matchApplicationRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserProfileImageService _profileImageService;

    public GetUserProfileRequestHandler(
        IUserRepository userRepository,
        IEventRepository eventRepository,
        IFriendshipRequestRepository friendshipRequestRepository,
        IMatchRepository matchRepository,
        IMatchApplicationRepository matchApplicationRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IUserProfileImageService profileImageService)
    {
        _userRepository = userRepository;
        _eventRepository = eventRepository;
        _friendshipRequestRepository = friendshipRequestRepository;
        _matchApplicationRepository = matchApplicationRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _profileImageService = profileImageService;
        _eventRepository = eventRepository;
        _matchRepository = matchRepository;
    }


    public async Task<UserProfileResponse> Handle(GetUserProfileInput request, CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        var maybeCurrentUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeCurrentUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var user = maybeUser.Value;
        var currentUser = maybeCurrentUser.Value;

        var areFriends = currentUser.IsFriendWith(user);

        var friendshipRequests = await _friendshipRequestRepository.GetAllPendingForUserIdAsync(currentUser.Id, cancellationToken);
        var pendingFriendshipRequest = friendshipRequests.FirstOrDefault(x => x.FriendId == user.Id || x.UserId == user.Id);
        var mutualFriends = await new FriendshipService(_userRepository).GetMutualFriends(currentUser, user, cancellationToken:cancellationToken);

        var userProfileType = new ExtendedUserType(user, areFriends, pendingFriendshipRequest, mutualFriends);

        await _profileImageService.PopulateImageUrl(userProfileType);

        var matchApplications = await _matchApplicationRepository.GetPendingByUserId(request.UserId, cancellationToken);
        var futureMatchesOfCurrentUser = await _matchRepository.GetFutureUserMatches(currentUser.Id, cancellationToken);
        var matchesMap = futureMatchesOfCurrentUser.ToLookup(x => x.Id);

        var matchApplicationsRelatedToCurrentUser = matchApplications
            .Where(x => matchesMap.Contains(x.MatchId))
            .Select(x => new { Match = matchesMap[x.MatchId].Single(), Application = x });
        
        var eventIdsThatUserIsAdminOn = await _eventRepository.GetEventIdsThatUserIsAdminOn(currentUser.Id, cancellationToken);
        var matchApplicationTypes = new List<MatchApplicationType>();

        foreach (var matchApplicationMap in matchApplicationsRelatedToCurrentUser)
        {
            var isCurrentUserAdminOnEvent = eventIdsThatUserIsAdminOn.Contains(matchApplicationMap.Match.EventId);
            matchApplicationTypes.Add(new MatchApplicationType(matchApplicationMap.Match, matchApplicationMap.Application, isCurrentUserAdminOnEvent));
        }

        var userProfileResponse = new UserProfileResponse
        {
            User = userProfileType,
            MatchApplications = matchApplicationTypes.ToList()
        };

        return userProfileResponse;
    }
}