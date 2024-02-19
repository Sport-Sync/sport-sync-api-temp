using Microsoft.Extensions.Options;
using SportSync.Application.Core.Settings;
using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Events.DomainEvents;

internal sealed class EventCreatedDomainEventHandler : IDomainEventHandler<EventCreatedDomainEvent>
{
    private readonly EventSettings _eventSettings;
    private readonly ITerminRepository _terminRepository;

    public EventCreatedDomainEventHandler(IOptions<EventSettings> eventSettings, ITerminRepository terminRepository)
    {
        _eventSettings = eventSettings.Value;
        _terminRepository = terminRepository;
    }

    public Task Handle(EventCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var @event = domainEvent.CreatedEvent;

        var terminsToCreate = new List<Termin>();

        foreach (var schedule in @event.Schedules)
        {
            if (schedule.RepeatWeekly)
            {
                var termins = AddWeeklyRepeatableTermins(@event, schedule, schedule.StartDate);
                terminsToCreate.AddRange(termins);
            }
            else
            {
                var termin = AddTermin(@event, schedule);
                terminsToCreate.Add(termin);
            }
        }

        _terminRepository.InsertRange(terminsToCreate);

        return Task.CompletedTask;
    }

    private Termin AddTermin(Event @event, EventSchedule schedule)
    {
        var termin = Termin.Create(@event, schedule.StartDate, schedule);

        termin.AddPlayers(@event.MemberUserIds);

        return termin;
    }

    private IEnumerable<Termin> AddWeeklyRepeatableTermins(Event @event, EventSchedule schedule, DateTime startDate)
    {
        var nextTerminDate = startDate;

        for (int i = 0; i < _eventSettings.NumberOfTerminsToCreateInFuture; i++)
        {
            var nextTermin = Termin.Create(@event, nextTerminDate, schedule);
            nextTermin.AddPlayers(@event.MemberUserIds);

            nextTerminDate = nextTerminDate.AddDays(7);

            yield return nextTermin;
        }
    }
}