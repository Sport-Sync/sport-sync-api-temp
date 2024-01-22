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
        var repeatableEventSchedules = await _eventRepository.GetAllRepeatableEventSchedules();

        foreach (var eventSchedule in repeatableEventSchedules)
        {
            var @event = eventSchedule.Event;

            var pendingTermins = @event.Termins.Where(t => t.Date > DateOnly.FromDateTime(DateTime.Today));

            if (!pendingTermins.Any())
            {
                _logger.LogError("No termins found to be played for event {eventId} (active and weekly repeatable)", @event.Id);
                continue;
            }

            var lastTermin = pendingTermins.OrderByDescending(t => t.Date).First();
            var terminsToCreate = _eventSettings.NumberOfTerminsToCreateInFuture - pendingTermins.Count();

            if (terminsToCreate <= 0)
            {
                continue;
            }

            @event.AddWeeklyRepeatableTermins(
                lastTermin.Date.AddDays(7), 
                eventSchedule.StartTimeUtc, 
                eventSchedule.EndTimeUtc, 
                terminsToCreate);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}