using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Core.Settings;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Infrastructure.Jobs;

public class CreateAdditionalMatchesJob : IJob
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAdditionalMatchesJob> _logger;
    private readonly EventSettings _eventSettings;

    public CreateAdditionalMatchesJob(
        IMatchRepository matchRepository,
        IOptions<EventSettings> eventSettings,
        ILogger<CreateAdditionalMatchesJob> logger,
        IUnitOfWork unitOfWork)
    {
        _matchRepository = matchRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _eventSettings = eventSettings.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting job '{name}'", nameof(CreateAdditionalMatchesJob));

        var matchesToCreate = new List<Match>();
        var repeatableEventSchedules = await _matchRepository.GetLastRepeatableMatches();

        foreach (var matchMap in repeatableEventSchedules)
        {
            if (!matchMap.LastMatch.Schedule.RepeatWeekly)
            {
                continue;
            }

            var nextMatchDate = matchMap.LastMatch.Date.AddDays(7);

            var amountOfMatchesToCreate = _eventSettings.NumberOfMatchesToCreateInFuture - matchMap.PendingMatchesCount;

            if (amountOfMatchesToCreate < 1)
            {
                continue;
            }

            for (int i = 0; i < amountOfMatchesToCreate; i++)
            {
                var nextMatch = Match.CreateCopy(matchMap.LastMatch, nextMatchDate);
                matchesToCreate.Add(nextMatch);
                nextMatchDate = nextMatchDate.AddDays(7);
            }
        }

        if (matchesToCreate.Any())
        {
            _matchRepository.InsertRange(matchesToCreate);
            await _unitOfWork.SaveChangesAsync();
        }

        _logger.LogInformation("Job '{name}' completed successfully", nameof(CreateAdditionalMatchesJob));
    }
}