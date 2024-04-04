using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.GetMatchById;

public class GetMatchByIdRequestHandler : IRequestHandler<GetMatchByIdInput, GetMatchByIdResponse>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IBlobStorageService _blobStorageService;

    public GetMatchByIdRequestHandler(IMatchRepository matchRepository, IUserIdentifierProvider userIdentifierProvider, IBlobStorageService blobStorageService)
    {
        _matchRepository = matchRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _blobStorageService = blobStorageService;
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

        var playersAttending = match.Players.Where(p => p.Attending == true).Select(PlayerType.FromPlayer).ToList();
        var playersNotAttending = match.Players.Where(p => p.Attending == false).Select(PlayerType.FromPlayer).ToList();
        var playersNotResponded = match.Players.Where(p => p.Attending == null).Select(PlayerType.FromPlayer).ToList();

        await PopulateImageUrls(playersAttending);
        await PopulateImageUrls(playersNotAttending);
        await PopulateImageUrls(playersNotResponded);

        var response = new GetMatchByIdResponse
        {
            Match = MatchType.FromMatch(match),
            IsCurrentUserAdmin = isCurrentUserAdmin,
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

    private async Task PopulateImageUrls(List<PlayerType> players)
    {
        foreach (var player in players)
        {
            var profileImage = player.HasProfileImage ? await _blobStorageService.GetProfileImageUrl(player.UserId) : null;
            player.ImageUrl = profileImage;
        }
    }
}