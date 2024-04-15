using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.CancelMatchApplication;

public class CancelMatchApplicationRequestHandler : IRequestHandler<CancelMatchApplicationInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IMatchApplicationRepository _matchApplicationRepository;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    public CancelMatchApplicationRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IMatchApplicationRepository matchApplicationRepository,
        IDateTime dateTime,
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _matchApplicationRepository = matchApplicationRepository;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelMatchApplicationInput request, CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        var maybeMatchApplication = await _matchApplicationRepository.GetByIdAsync(request.MatchApplicationId, cancellationToken);
        if (maybeMatchApplication.HasNoValue)
        {
            return Result.Failure(DomainErrors.MatchApplication.NotFound);
        }

        var matchApplication = maybeMatchApplication.Value;
        var user = maybeUser.Value;

        var cancelResult = matchApplication.Cancel(user, _dateTime.UtcNow);

        if (cancelResult.IsFailure)
        {
            return Result.Failure(cancelResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}