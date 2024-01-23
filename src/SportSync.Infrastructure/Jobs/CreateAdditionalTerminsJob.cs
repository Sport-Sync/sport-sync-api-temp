using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Core.Settings;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Infrastructure.Jobs;

public class CreateAdditionalTerminsJob : IJob
{
    private readonly ITerminRepository _terminRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAdditionalTerminsJob> _logger;
    private readonly EventSettings _eventSettings;

    public CreateAdditionalTerminsJob(
        ITerminRepository terminRepository,
        IOptions<EventSettings> eventSettings,
        ILogger<CreateAdditionalTerminsJob> logger,
        IUnitOfWork unitOfWork)
    {
        _terminRepository = terminRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _eventSettings = eventSettings.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting job '{name}'", nameof(CreateAdditionalTerminsJob));

        var terminsToCreate = new List<Termin>();
        var repeatableEventSchedules = await _terminRepository.GetLastRepeatableTermins();

        foreach (var terminMap in repeatableEventSchedules)
        {
            var nextTerminDate = terminMap.LastTermin.Date.AddDays(7);

            var amountOfTerminsToCreate = _eventSettings.NumberOfTerminsToCreateInFuture - terminMap.PendingTerminsCount;

            if (amountOfTerminsToCreate < 1)
            {
                continue;
            }

            for (int i = 0; i < amountOfTerminsToCreate; i++)
            {
                var nextTermin = Termin.CreateCopy(terminMap.LastTermin, nextTerminDate);
                terminsToCreate.Add(nextTermin);
                nextTerminDate = nextTerminDate.AddDays(7);
            }
        }

        _terminRepository.InsertRange(terminsToCreate);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Job '{name}' completed successfully", nameof(CreateAdditionalTerminsJob));
    }
}