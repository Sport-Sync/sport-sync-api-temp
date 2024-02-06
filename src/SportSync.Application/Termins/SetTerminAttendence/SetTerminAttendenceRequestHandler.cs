using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Termins.SetTerminAttendence;

public class SetTerminAttendenceRequestHandler : IRequestHandler<SetTerminAttendenceInput, SetTerminAttendenceResponse>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ITerminRepository _terminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetTerminAttendenceRequestHandler(IUserIdentifierProvider userIdentifierProvider, ITerminRepository terminRepository, IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _terminRepository = terminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SetTerminAttendenceResponse> Handle(SetTerminAttendenceInput input, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeTermin = await _terminRepository.GetByIdAsync(input.TerminId, cancellationToken);

        if (maybeTermin.HasNoValue)
        {
            throw new DomainException(DomainErrors.Termin.NotFound);
        }

        var termin = maybeTermin.Value;
        termin.SetPlayerAttendence(userId, input.Attending);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var players = await _terminRepository
            .GetPlayers(input.TerminId, cancellationToken);

        return new SetTerminAttendenceResponse
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