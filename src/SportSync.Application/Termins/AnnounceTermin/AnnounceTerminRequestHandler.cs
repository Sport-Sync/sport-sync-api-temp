using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Termins.AnnounceTermin;

public class AnnounceTerminRequestHandler : IRequestHandler<AnnounceTerminInput, TerminType>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ITerminRepository _terminRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnnounceTerminRequestHandler(
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

    public async Task<TerminType> Handle(AnnounceTerminInput input, CancellationToken cancellationToken)
    {
        var maybeTermin = await _terminRepository.GetByIdAsync(input.TerminId, cancellationToken);

        if (maybeTermin.HasNoValue)
        {
            throw new DomainException(DomainErrors.Termin.NotFound);
        }

        var termin = maybeTermin.Value;
        var currentUserId = _userIdentifierProvider.UserId;

        await _eventRepository.EnsureUserIsAdminOnEvent(termin.EventId, currentUserId, cancellationToken);

        termin.Announce(currentUserId, input.PublicAnnouncement);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return TerminType.FromTermin(termin);
    }
}