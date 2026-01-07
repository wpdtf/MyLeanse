using MyLeanse.CallbackService;
using MyLeanse.Infrastructure;
using MyLeanse.LocalDatabase;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

var configurate = builder.Configuration;

builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<BotService>();
builder.Services.AddSingleton<MessageCallback>();
builder.Services.AddSingleton<KeyboardCallback>();
builder.Services.AddSingleton<LeanseStorage>();
builder.Services.AddSingleton<MessageSendAsync>();

builder.Services.AddQuartz(q =>
{
    q.AddJob<DailyJob>(opts => opts.WithIdentity("dailyJob"));

    q.AddTrigger(opts => opts
        .ForJob("dailyJob")
    .WithIdentity("daily3amTrigger")
        .WithCronSchedule(configurate.GetValue<string>("Cron") ?? "0 0 3 * * ? *")
    );
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

var host = builder.Build();
host.Run();
