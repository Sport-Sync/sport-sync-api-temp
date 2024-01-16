using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.ValueObjects;

namespace SportSync.Application.Events.CreateEvent;

public class CreateEventInputHandler : IInputHandler<CreateEventInput, Guid>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventInputHandler(
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateEventInput request, CancellationToken cancellationToken)
    {
        var creatorId = _userIdentifierProvider.UserId;

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(creatorId);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        var user = maybeUser.Value;

        var eventTimes = request.EventTime.Select(time => EventTime.Create(
           time.DayOfWeek,
           TimeOnly.FromDateTime(time.StartTime.UtcDateTime),
           TimeOnly.FromDateTime(time.EndTime.UtcDateTime),
           time.RepeatWeekly));

        var @event = user.CreateEvent(request.Name, request.SportType, request.Address, request.Price,
            request.NumberOfPlayers, eventTimes, request.Notes);

        foreach (var memberId in request.MemberIds)
        {
            @event.AddMember(memberId);
        }

        _eventRepository.Insert(@event);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.Id;
    }
}