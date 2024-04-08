using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.CancelMatchAnnouncement;

public class CancelMatchAnnouncementRequestHandler : IRequestHandler<CancelMatchAnnouncementInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IMatchRepository _matchRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelMatchAnnouncementRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IMatchRepository matchRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelMatchAnnouncementInput request, CancellationToken cancellationToken)
    {
        var maybeMatch = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
        if (maybeMatch.HasNoValue)
        {
            return Result.Failure(DomainErrors.Match.NotFound);
        }

        var match = maybeMatch.Value;

        var result = await match.CancelAnnouncement(_userIdentifierProvider.UserId, _eventRepository, cancellationToken);

        if (!result.IsFailure)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}