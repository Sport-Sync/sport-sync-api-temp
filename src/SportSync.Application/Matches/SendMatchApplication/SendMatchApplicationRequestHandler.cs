using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.SendMatchApplication;

public class SendMatchApplicationRequestHandler : IRequestHandler<SendMatchApplicationInput, Result>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchApplicationRepository _matchApplicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUnitOfWork _unitOfWork;

    public SendMatchApplicationRequestHandler(
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IMatchApplicationRepository matchApplicationRepository,
        IUserIdentifierProvider userIdentifierProvider, 
        IUnitOfWork unitOfWork)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _matchApplicationRepository = matchApplicationRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendMatchApplicationInput request, CancellationToken cancellationToken)
    {
        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        var maybeMatch = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            return Result.Failure(DomainErrors.Match.NotFound);
        }

        var existingMatchApplications = await _matchApplicationRepository.GetPendingByMatchId(request.MatchId, cancellationToken);
        if (existingMatchApplications.Any(a => a.AppliedByUserId == _userIdentifierProvider.UserId))
        {
            return Result.Failure(DomainErrors.MatchApplication.PendingMatchApplication);
        }

        var match = maybeMatch.Value;
        var user = maybeUser.Value;

        var application = match.ApplyForPlaying(user);

        if (application.IsFailure)
        {
            return Result.Failure(application.Error);
        }

        _matchApplicationRepository.Insert(application.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}