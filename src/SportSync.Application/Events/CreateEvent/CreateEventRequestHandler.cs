using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Events.CreateEvent;

public class CreateEventRequestHandler : IRequestHandler<CreateEventInput, Guid>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventRequestHandler(
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

        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(creatorId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.NotFound);
        }

        var user = maybeUser.Value;

        var @event = user.CreateEvent(
            request.Name, request.SportType, request.Address, request.Price, request.NumberOfPlayers, request.Notes, request.MemberIds.ToArray());

        var eventSchedules = request.EventTime.Select(time => EventSchedule.Create(
            time.DayOfWeek,
            time.StartDate,
            time.StartTime,
            time.EndTime,
            time.RepeatWeekly)).ToList();

        @event.AddMembers(request.MemberIds.ToArray());
        @event.AddSchedules(eventSchedules);

        _eventRepository.Insert(@event);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.Id;
    }
}