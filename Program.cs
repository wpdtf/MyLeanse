using MyLeanse.CallbackService;
using MyLeanse.Infrastructure;
using MyLeanse.LocalDatabase;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<BotService>();
builder.Services.AddSingleton<MessageCallback>();
builder.Services.AddSingleton<KeyboardCallback>();
builder.Services.AddSingleton<LeanseStorage>();
builder.Services.AddSingleton<MessageSendAsync>();

var host = builder.Build();
host.Run();
