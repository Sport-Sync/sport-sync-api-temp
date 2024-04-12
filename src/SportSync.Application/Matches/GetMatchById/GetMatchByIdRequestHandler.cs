using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Services;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.GetMatchById;

public class GetMatchByIdRequestHandler : IRequestHandler<GetMatchByIdInput, GetMatchByIdResponse>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchApplicationRepository _matchApplicationRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserProfileImageService _userProfileImageService;

    public GetMatchByIdRequestHandler(
        IMatchRepository matchRepository,
        IMatchApplicationRepository matchApplicationRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IUserProfileImageService userProfileImageService)
    {
        _matchRepository = matchRepository;
        _matchApplicationRepository = matchApplicationRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _userProfileImageService = userProfileImageService;
    }

    public async Task<GetMatchByIdResponse> Handle(GetMatchByIdInput request, CancellationToken cancellationToken)
    {
        var maybeMatch = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            throw new DomainException(DomainErrors.Match.NotFound);
        }

        var match = maybeMatch.Value;
        var currentUserId = _userIdentifierProvider.UserId;

        if (!match.IsPlayer(currentUserId))
        {
            throw new DomainException(DomainErrors.Match.PlayerNotFound);
        }

        var admins = await _matchRepository.GetAdmins(match.Id, cancellationToken);

        var isCurrentUserAdmin = admins.Any(a => a.UserId == currentUserId);
        var isCurrentUserAttending = match.Players.First(p => p.UserId == currentUserId).Attending;

        var playersAttending = match.Players.Where(p => p.Attending == true).Select(PlayerType.FromPlayer).ToArray();
        var playersNotAttending = match.Players.Where(p => p.Attending == false).Select(PlayerType.FromPlayer).ToArray();
        var playersNotResponded = match.Players.Where(p => p.Attending == null).Select(PlayerType.FromPlayer).ToArray();

        var matchApplications = await _matchApplicationRepository.GetPendingByMatchIdWithIncludedUser(match.Id, cancellationToken);
        var pendingApplicants = matchApplications
            .Select(x => new UserType(x.AppliedByUser))
            .ToArray();

        await _userProfileImageService.PopulateImageUrl(playersAttending);
        await _userProfileImageService.PopulateImageUrl(playersNotAttending);
        await _userProfileImageService.PopulateImageUrl(playersNotResponded);
        await _userProfileImageService.PopulateImageUrl(pendingApplicants);

        var response = new GetMatchByIdResponse
        {
            Match = MatchType.FromMatch(match),
            IsCurrentUserAdmin = isCurrentUserAdmin,
            PendingApplicants = pendingApplicants.ToList(),
            Attendance = new MatchAttendanceType()
            {
                IsCurrentUserAttending = isCurrentUserAttending,
                PlayersAttending = playersAttending.ToList(),
                PlayersNotAttending = playersNotAttending.ToList(),
                PlayersNotResponded = playersNotResponded.ToList(),
            }
        };

        return response;
    }
}