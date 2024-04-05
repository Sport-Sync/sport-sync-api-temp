using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.GetAnnouncedMatches;

public class GetAnnouncedMatchesRequestHandler : IRequestHandler<GetAnnouncedMatchesInput, GetAnnouncedMatchResponse>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetAnnouncedMatchesRequestHandler(
        IMatchRepository matchRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
    }

    public async Task<GetAnnouncedMatchResponse> Handle(GetAnnouncedMatchesInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var announcedMatches = await _matchRepository.GetAnnouncedMatches(request.Date, cancellationToken);

        var user = maybeUser.Value;
        var response = new GetAnnouncedMatchResponse();

        if (!announcedMatches.Any())
        {
            return response;
        }

        var publicAnnouncements = announcedMatches.Where(x => x.PubliclyAnnounced);
        var privateAnnouncements = announcedMatches.Where(x => !x.PubliclyAnnounced);

        response.Matches.AddRange(publicAnnouncements.Select(MatchType.FromMatch));

        foreach (var match in privateAnnouncements)
        {
            var announcingUserIds = match.Players.Where(x => x.HasAnnouncedMatch).Select(x => x.UserId);
            if (user.Friends.Any(friendId => announcingUserIds.Contains(friendId)))
            {
                response.Matches.Add(MatchType.FromMatch(match));
            }
        }

        return response;
    }
}