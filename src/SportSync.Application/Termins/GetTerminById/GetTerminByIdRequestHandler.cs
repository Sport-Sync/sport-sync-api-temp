using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Termins.GetTerminById;

public class GetTerminByIdRequestHandler : IRequestHandler<GetTerminByIdInput, GetTerminByIdResponse>
{
    private readonly ITerminRepository _terminRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetTerminByIdRequestHandler(ITerminRepository terminRepository, IUserIdentifierProvider userIdentifierProvider)
    {
        _terminRepository = terminRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<GetTerminByIdResponse> Handle(GetTerminByIdInput request, CancellationToken cancellationToken)
    {
        var maybeTermin = await _terminRepository.GetByIdAsync(request.TerminId, cancellationToken);

        if (maybeTermin.HasNoValue)
        {
            throw new DomainException(DomainErrors.Termin.NotFound);
        }

        var termin = maybeTermin.Value;
        var currentUserId = _userIdentifierProvider.UserId;

        if (!termin.IsPlayer(currentUserId))
        {
            throw new DomainException(DomainErrors.Termin.PlayerNotFound);
        }

        var isCurrentUserAttending = termin.Players.First(p => p.UserId == currentUserId).Attending;
        var playersAttending = termin.Players.Where(p => p.Attending == true && p.UserId != currentUserId);
        var playersNotAttending = termin.Players.Where(p => p.Attending == false && p.UserId != currentUserId);
        var playersNotResponded = termin.Players.Where(p => p.Attending == null && p.UserId != currentUserId);

        var response = new GetTerminByIdResponse
        {
            Termin = TerminType.FromTermin(termin),
            Attendence = new TerminAttendenceType()
            {
                IsCurrentUserAttending = isCurrentUserAttending,
                PlayersAttending = playersAttending.Select(PlayerType.FromPlayer).ToList(),
                PlayersNotAttending = playersNotAttending.Select(PlayerType.FromPlayer).ToList(),
                PlayersNotResponded = playersNotResponded.Select(PlayerType.FromPlayer).ToList(),
            }
        };

        return response;
    }
}