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
    private readonly IUserRepository _userRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnnounceMatchRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _matchRepository = matchRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<MatchType> Handle(AnnounceMatchInput request, CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var maybeMatch = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            throw new DomainException(DomainErrors.Match.NotFound);
        }

        var match = maybeMatch.Value;
        var user = maybeUser.Value;

        if (request.PublicAnnouncement)
        {
            await _eventRepository.EnsureUserIsAdminOnEvent(match.EventId, user.Id, cancellationToken);
        }

        match.Announce(user, request.PublicAnnouncement, request.PlayerLimit, request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MatchType.FromMatch(match);
    }
}