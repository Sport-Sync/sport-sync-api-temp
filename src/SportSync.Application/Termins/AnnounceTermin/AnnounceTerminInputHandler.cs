using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Termins.AnnounceTermin;

public class AnnounceTerminInputHandler : IInputHandler<AnnounceTerminInput, TerminType>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ITerminRepository _terminRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnnounceTerminInputHandler(
        ITerminRepository terminRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _terminRepository = terminRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TerminType> Handle(AnnounceTerminInput request, CancellationToken cancellationToken)
    {
        var maybeTermin = await _terminRepository.GetByIdAsync(request.TerminId, cancellationToken);

        if (maybeTermin.HasNoValue)
        {
            throw new DomainException(DomainErrors.Termin.NotFound);
        }

        var termin = maybeTermin.Value;
        var currentUserId = _userIdentifierProvider.UserId;

        await _eventRepository.EnsureUserIsAdminOnEvent(termin.EventId, currentUserId, cancellationToken);

        termin.Announce(request.PublicAnnouncement);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TerminType.FromTermin(termin);
    }
}