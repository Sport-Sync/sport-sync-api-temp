using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using MatchType = SportSync.Domain.Types.MatchType;

namespace SportSync.Application.Matches.AnnounceMatch;

public class AnnounceMatchRequestHandler : IRequestHandler<AnnounceMatchInput, MatchType>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IMatchRepository _matchRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnnounceMatchRequestHandler(
        IMatchRepository matchRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _matchRepository = matchRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MatchType> Handle(AnnounceMatchInput input, CancellationToken cancellationToken)
    {
        var maybeMatch = await _matchRepository.GetByIdAsync(input.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            throw new DomainException(DomainErrors.Match.NotFound);
        }

        var match = maybeMatch.Value;
        var currentUserId = _userIdentifierProvider.UserId;

        if (input.PublicAnnouncement)
        {
            await _eventRepository.EnsureUserIsAdminOnEvent(match.EventId, currentUserId, cancellationToken);
        }

        match.Announce(currentUserId, input.PublicAnnouncement);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MatchType.FromMatch(match);
    }
}