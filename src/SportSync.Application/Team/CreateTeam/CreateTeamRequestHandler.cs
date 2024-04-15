using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Team.CreateTeam;

public class CreateTeamRequestHandler : IRequestHandler<CreateTeamInput, TeamType>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeamRequestHandler(IUserIdentifierProvider userIdentifierProvider, IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TeamType> Handle(CreateTeamInput request, CancellationToken cancellationToken)
    {
        var maybeEvent = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (maybeEvent.HasNoValue)
        {
            throw new DomainException(DomainErrors.Event.NotFound);
        }

        var @event = maybeEvent.Value;

        var team = @event.AddTeam(_userIdentifierProvider.UserId, request.Name);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TeamType.FromTeam(team);
    }
}