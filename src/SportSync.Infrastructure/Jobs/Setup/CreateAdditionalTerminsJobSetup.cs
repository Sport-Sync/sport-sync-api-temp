using Microsoft.Extensions.Options;
using Quartz;

namespace SportSync.Infrastructure.Jobs.Setup;

internal class CreateAdditionalTerminsJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(CreateAdditionalTerminsJob));

        options.AddJob<CreateAdditionalTerminsJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(jobKey)
                    .WithDailyTimeIntervalSchedule(
                        s => s.WithIntervalInHours(24)
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                            .InTimeZone(TimeZoneInfo.Utc)));
    }
}