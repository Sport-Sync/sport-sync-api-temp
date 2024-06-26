﻿using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.GetAnnouncedMatches;

public class GetAnnouncedMatchesRequestHandler : IRequestHandler<GetAnnouncedMatchesInput, GetAnnouncedMatchesResponse>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchApplicationRepository _matchApplicationRepository;
    private readonly IUserRepository _userRepository;

    public GetAnnouncedMatchesRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IMatchRepository matchRepository,
        IMatchApplicationRepository matchApplicationRepository,
        IUserRepository userRepository)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _matchRepository = matchRepository;
        _matchApplicationRepository = matchApplicationRepository;
        _userRepository = userRepository;
    }

    public async Task<GetAnnouncedMatchesResponse> Handle(GetAnnouncedMatchesInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var announcedMatches = await _matchRepository.GetAnnouncedMatches(request.Date, cancellationToken);

        var user = maybeUser.Value;
        var response = new GetAnnouncedMatchesResponse();

        if (!announcedMatches.Any())
        {
            return response;
        }

        var userApplications = await _matchApplicationRepository.GetPendingByUserId(userId, cancellationToken);
        var userApplicationMap = userApplications.ToLookup(x => x.MatchId);

        var publicAnnouncements = announcedMatches.Where(x => x.PubliclyAnnounced);
        var privateAnnouncements = announcedMatches.Where(x => !x.PubliclyAnnounced);

        response.Matches.AddRange(publicAnnouncements.Select(x => 
            new MatchAnnouncementType(x, userApplicationMap.Contains(x.Id), x.IsPlayer(userId))));

        foreach (var match in privateAnnouncements)
        {
            if (match.IsPlayer(userId))
            {
                response.Matches.Add(new MatchAnnouncementType(match, userApplicationMap.Contains(match.Id), true));
                continue;
            }

            var announcingUserIds = match.Players.Where(x => x.HasAnnouncedMatch).Select(x => x.UserId);
            if (user.Friends.Any(friendId => announcingUserIds.Contains(friendId)))
            {
                response.Matches.Add(new MatchAnnouncementType(match, userApplicationMap.Contains(match.Id), false));
            }
        }

        return response;
    }
}