using Quartz;

namespace MyLeanse.Infrastructure;

public static class Dependencies
{
    public static IServiceCollection SetQuartz(this IServiceCollection services)
    {

        services.AddQuartz(q =>
        {
            q.AddJob<DailyJob>(opts => opts.WithIdentity("dailyJob"));

            q.AddTrigger(opts => opts
                .ForJob("dailyJob")
            .WithIdentity("daily3amTrigger")
                .WithCronSchedule("0 0 3 * * ? *")
            );
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
