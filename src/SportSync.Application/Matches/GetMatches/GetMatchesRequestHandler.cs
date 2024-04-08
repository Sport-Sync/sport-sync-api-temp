using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Repositories;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.GetMatches;

public class GetMatchesRequestHandler : IRequestHandler<GetMatchesInput, GetMatchesResponse>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetMatchesRequestHandler(IMatchRepository matchRepository, IUserIdentifierProvider userIdentifierProvider)
    {
        _matchRepository = matchRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<GetMatchesResponse> Handle(GetMatchesInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;
        var matches = await _matchRepository.GetUserMatchesOnDate(request.Date, userId, cancellationToken);

        return new GetMatchesResponse
        {
            Matches = matches.Select(MatchType.FromMatch).ToList()
        };
    }
}