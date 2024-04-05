using Microsoft.Extensions.Options;
using SportSync.Application.Core.Settings;
using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.DomainEvents;

public class CreateMatchesOnEventCreatedHandler : IDomainEventHandler<EventCreatedDomainEvent>
{
    private readonly EventSettings _eventSettings;
    private readonly IMatchRepository _matchRepository;

    public CreateMatchesOnEventCreatedHandler(IOptions<EventSettings> eventSettings, IMatchRepository matchRepository)
    {
        _eventSettings = eventSettings.Value;
        _matchRepository = matchRepository;
    }

    public Task Handle(EventCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var @event = domainEvent.CreatedEvent;

        var matchesToCreate = new List<Match>();

        foreach (var schedule in @event.Schedules)
        {
            if (schedule.RepeatWeekly)
            {
                var matches = AddWeeklyRepeatableMatches(@event, schedule, schedule.StartDate);
                matchesToCreate.AddRange(matches);
            }
            else
            {
                var match = AddMatch(@event, schedule);
                matchesToCreate.Add(match);
            }
        }

        _matchRepository.InsertRange(matchesToCreate);

        return Task.CompletedTask;
    }

    private Match AddMatch(Event @event, EventSchedule schedule)
    {
        var match = Match.Create(@event, schedule.StartDate, schedule);

        match.AddPlayers(@event.MemberUserIds);

        return match;
    }

    private IEnumerable<Match> AddWeeklyRepeatableMatches(Event @event, EventSchedule schedule, DateTime startDate)
    {
        var nextMatchDate = startDate;

        for (int i = 0; i < _eventSettings.NumberOfMatchesToCreateInFuture; i++)
        {
            var nextMatch = Match.Create(@event, nextMatchDate, schedule);
            nextMatch.AddPlayers(@event.MemberUserIds);

            nextMatchDate = nextMatchDate.AddDays(7);

            yield return nextMatch;
        }
    }
}