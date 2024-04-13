using Microsoft.Extensions.Options;
using Quartz;

namespace SportSync.Infrastructure.Jobs.Setup;

internal class BackgroundJobsSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var createAdditionalJobKey = JobKey.Create(nameof(CreateAdditionalMatchesJob));
        options.AddJob<CreateAdditionalMatchesJob>(jobBuilder => jobBuilder.WithIdentity(createAdditionalJobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(createAdditionalJobKey)
                    .WithDailyTimeIntervalSchedule(
                        s => s.WithIntervalInHours(24)
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(3, 0))
                            .InTimeZone(TimeZoneInfo.Utc)));

        var updateMatchStatusJobKey = JobKey.Create(nameof(UpdateMatchStatusJob));
        options.AddJob<UpdateMatchStatusJob>(jobBuilder => jobBuilder.WithIdentity(updateMatchStatusJobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(updateMatchStatusJobKey)
                    .WithDailyTimeIntervalSchedule(
                        s => s.WithIntervalInMinutes(15)
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 1))
                            .InTimeZone(TimeZoneInfo.Utc)));
    }
}