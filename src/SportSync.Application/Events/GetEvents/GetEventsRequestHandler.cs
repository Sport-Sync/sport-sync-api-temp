using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Events.GetEvents;

public class GetEventsRequestHandler : IRequestHandler<List<EventType>>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IEventRepository _eventRepository;

    public GetEventsRequestHandler(IUserIdentifierProvider userIdentifierProvider, IEventRepository eventRepository)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _eventRepository = eventRepository;
    }

    public async Task<List<EventType>> Handle(CancellationToken cancellationToken)
    {
        var events = await _eventRepository.GetEventsByUserId(_userIdentifierProvider.UserId, cancellationToken);

        return events.Select(EventType.FromEvent).ToList();
    }
}