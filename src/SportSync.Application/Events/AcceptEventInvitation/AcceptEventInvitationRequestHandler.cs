using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Events.Common;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Events.AcceptEventInvitation;

public class AcceptEventInvitationRequestHandler : IRequestHandler<EventInvitationInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public AcceptEventInvitationRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IEventRepository eventRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(EventInvitationInput request, CancellationToken cancellationToken)
    {
        var maybeEvent = await _eventRepository.GetByEventInvitationIdAsync(request.EventInvitationId, cancellationToken);

        if (maybeEvent.HasNoValue)
        {
            return Result.Failure(DomainErrors.EventInvitation.NotFound);
        }

        var maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var @event = maybeEvent.Value;
        var user = maybeUser.Value;

        Result acceptResult = @event.AcceptInvitation(request.EventInvitationId, user, _dateTime.UtcNow);

        if (acceptResult.IsFailure)
        {
            return Result.Failure(acceptResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}