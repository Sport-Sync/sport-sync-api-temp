using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Infrastructure.Jobs;

public class UpdateMatchStatusJob : IJob
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchApplicationRepository _matchApplicationRepository;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMatchStatusJob> _logger;

    public UpdateMatchStatusJob(
        IMatchRepository matchRepository,
        IMatchApplicationRepository matchApplicationRepository,
        IDateTime dateTime,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMatchStatusJob> logger)
    {
        _matchRepository = matchRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dateTime = dateTime;
        _matchApplicationRepository = matchApplicationRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting job '{name}'", nameof(UpdateMatchStatusJob));

        var now = _dateTime.UtcNow;

        var matches = _matchRepository.Where(x =>
            x.Date.Date == now.Date &&
            (x.Status == MatchStatusEnum.Pending || x.Status == MatchStatusEnum.InProgress))
            .Include(x => x.Announcement)
            .ToList();

        if (!matches.Any())
        {
            return;
        }

        var matchIds = matches.Select(m => m.Id).ToList();
        var matchApplications = await _matchApplicationRepository.GetPendingByMatchesIds(matchIds, CancellationToken.None);
        var matchApplicationsMap = matchApplications.ToLookup(x => x.MatchId);

        var pendingMatches = matches.Where(x => x.Status == MatchStatusEnum.Pending).ToList();
        var inProgressMatches = matches.Where(x => x.Status == MatchStatusEnum.InProgress).ToList();

        foreach (var pendingMatch in pendingMatches)
        {
            if (pendingMatch.StartTime.UtcDateTime <= now.AddMinutes(5))
            {
                pendingMatch.SetStatus(MatchStatusEnum.InProgress);
                pendingMatch.RemoveAnnouncement();

                foreach (var matchApplication in matchApplicationsMap[pendingMatch.Id])
                {
                    _matchApplicationRepository.Remove(matchApplication);
                }
            }
        }

        foreach (var inProgressMatch in inProgressMatches)
        {
            if (inProgressMatch.EndTime.UtcDateTime < now)
            {
                inProgressMatch.SetStatus(MatchStatusEnum.Finished);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Job '{name}' completed successfully", nameof(UpdateMatchStatusJob));
    }
}