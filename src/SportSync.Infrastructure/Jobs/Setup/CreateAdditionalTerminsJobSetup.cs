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
                    .WithSimpleSchedule(schedule =>
                        schedule.WithInterval(new TimeSpan(7, 0, 0, 0)).RepeatForever()));
    }
}