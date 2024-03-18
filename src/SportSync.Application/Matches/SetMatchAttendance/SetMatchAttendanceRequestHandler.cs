using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Matches.SetMatchAttendance;

public class SetMatchAttendanceRequestHandler : IRequestHandler<SetMatchAttendanceInput, SetMatchAttendanceResponse>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IMatchRepository _matchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetMatchAttendanceRequestHandler(IUserIdentifierProvider userIdentifierProvider, IMatchRepository matchRepository, IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _matchRepository = matchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SetMatchAttendanceResponse> Handle(SetMatchAttendanceInput input, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeMatch = await _matchRepository.GetByIdAsync(input.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            throw new DomainException(DomainErrors.Match.NotFound);
        }

        var match = maybeMatch.Value;
        match.SetPlayerAttendance(userId, input.Attending);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var players = await _matchRepository
            .GetPlayers(input.MatchId, cancellationToken);

        return new SetMatchAttendanceResponse
        {
            Players = players.Select(p => new PlayerType
            {
                UserId = p.UserId,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                IsAttending = p.Attending
            }).ToList()
        };
    }
}