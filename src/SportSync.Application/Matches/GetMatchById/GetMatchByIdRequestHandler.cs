﻿using SportSync.Application.Core.Abstractions.Authentication;
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

    public GetMatchByIdRequestHandler(IMatchRepository matchRepository, IUserIdentifierProvider userIdentifierProvider)
    {
        _matchRepository = matchRepository;
        _userIdentifierProvider = userIdentifierProvider;
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
        var playersAttending = match.Players.Where(p => p.Attending == true);
        var playersNotAttending = match.Players.Where(p => p.Attending == false);
        var playersNotResponded = match.Players.Where(p => p.Attending == null);

        var response = new GetMatchByIdResponse
        {
            Match = MatchType.FromMatch(match),
            IsCurrentUserAdmin = isCurrentUserAdmin,
            Attendance = new MatchAttendanceType()
            {
                IsCurrentUserAttending = isCurrentUserAttending,
                PlayersAttending = playersAttending.Select(PlayerType.FromPlayer).ToList(),
                PlayersNotAttending = playersNotAttending.Select(PlayerType.FromPlayer).ToList(),
                PlayersNotResponded = playersNotResponded.Select(PlayerType.FromPlayer).ToList(),
            }
        };

        return response;
    }
}