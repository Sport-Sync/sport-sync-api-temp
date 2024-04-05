using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Matches.Common;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.AcceptMatchApplication;

public class AcceptMatchApplicationRequestHandler : IRequestHandler<MatchApplicationInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IMatchApplicationRepository _matchApplicationRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public AcceptMatchApplicationRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IMatchApplicationRepository matchApplicationRepository,
        IMatchRepository matchRepository,
        IEventRepository eventRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _matchApplicationRepository = matchApplicationRepository;
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(MatchApplicationInput request, CancellationToken cancellationToken)
    {
        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        Maybe<MatchApplication> maybeMatchApplication = await _matchApplicationRepository.GetByIdAsync(request.MatchApplicationId, cancellationToken);

        if (maybeMatchApplication.HasNoValue)
        {
            return Result.Failure(DomainErrors.MatchApplication.NotFound);
        }

        Maybe<Match> maybeMatch = await _matchRepository.GetByIdAsync(maybeMatchApplication.Value.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            return Result.Failure(DomainErrors.MatchApplication.NotFound);
        }

        var matchApplication = maybeMatchApplication.Value;
        var match = maybeMatch.Value;
        var user = maybeUser.Value;

        await _eventRepository.EnsureUserIsAdminOnEvent(match.EventId, user.Id, cancellationToken);

        var acceptResult = matchApplication.Accept(user, match, _dateTime.UtcNow);

        if (acceptResult.IsFailure)
        {
            return Result.Failure(acceptResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}