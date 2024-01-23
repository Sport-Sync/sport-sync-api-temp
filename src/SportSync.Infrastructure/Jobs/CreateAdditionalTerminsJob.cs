using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Core.Settings;
using SportSync.Domain.Repositories;

namespace SportSync.Infrastructure.Jobs;

public class CreateAdditionalTerminsJob : IJob
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAdditionalTerminsJob> _logger;
    private readonly EventSettings _eventSettings;

    public CreateAdditionalTerminsJob(
        IEventRepository eventRepository,
        IOptions<EventSettings> eventSettings,
        ILogger<CreateAdditionalTerminsJob> logger,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _eventSettings = eventSettings.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting job '{name}'", nameof(CreateAdditionalTerminsJob));

        var repeatableEventSchedules = await _eventRepository.GetAllRepeatableEventSchedules();
        var eventIds = repeatableEventSchedules.Select(x => x.Event.Id);
        var eventsMap = (await _eventRepository.GetAllByIds(eventIds.ToList())).ToLookup(ev => ev.Id);

        foreach (var eventSchedule in repeatableEventSchedules)
        {
            var @event = eventSchedule.Event;
            var lastTermin = eventSchedule.LastTermin;
            var schedule = eventSchedule.Schedule;

            var terminsToCreate = _eventSettings.NumberOfTerminsToCreateInFuture - eventSchedule.PendingTerminsCount;

            if (terminsToCreate <= 0)
            {
                continue;
            }

            var ev = eventsMap[eventSchedule.Event.Id].First();

            //ev.AddWeeklyRepeatableTermins(
            //    lastTermin.Date.AddDays(7),
            //    schedule,
            //    terminsToCreate);
        }

        //await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Job '{name}' completed successfully", nameof(CreateAdditionalTerminsJob));
    }
}