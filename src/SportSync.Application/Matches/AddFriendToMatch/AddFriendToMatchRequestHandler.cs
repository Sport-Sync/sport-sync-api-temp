using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.AddFriendToMatch;

public class AddFriendToMatchRequestHandler : IRequestHandler<AddFriendToMatchInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddFriendToMatchRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddFriendToMatchInput request, CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var maybeMatch = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            return Result.Failure(DomainErrors.Match.NotFound);
        }

        var user = maybeUser.Value;
        var match = maybeMatch.Value;

        var result = user.AddFriendToMatch(request.FriendId, match);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}