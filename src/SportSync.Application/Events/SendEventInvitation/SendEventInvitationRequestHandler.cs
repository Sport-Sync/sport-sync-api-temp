using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Events.SendEventInvitation;

public class SendEventInvitationRequestHandler : IRequestHandler<SendEventInvitationInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendEventInvitationRequestHandler(
        IUserIdentifierProvider userIdentifierProvider, 
        IEventRepository eventRepository, 
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendEventInvitationInput request, CancellationToken cancellationToken)
    {
        var maybeEvent = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);

        if (maybeEvent.HasNoValue)
        {
            return Result.Failure(DomainErrors.Event.NotFound);
        }

        var maybeCurrentUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeCurrentUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var maybeInvitee = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (maybeInvitee.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var @event = maybeEvent.Value;
        var currentUser = maybeCurrentUser.Value;
        var invitee = maybeInvitee.Value;

        var invitationResult = await @event.InviteUser(currentUser, invitee, _eventRepository);

        if (invitationResult.IsFailure)
        {
            return Result.Failure(invitationResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}