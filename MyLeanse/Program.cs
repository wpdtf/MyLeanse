using MyLeanse.Handlers;
using MyLeanse.Infrastructure;
using MyLeanse.LocalDatabase;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SetQuartz();

builder.Services.SetCommand();
builder.Services.SetAction();

builder.Services.AddSingleton<UpdateDispatcher>();
builder.Services.AddSingleton<BotHost>();
builder.Services.AddSingleton<MessageHandler>();
builder.Services.AddSingleton<LeanseStorage>();
builder.Services.AddSingleton<MessageSendAsync>();

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var token = builder.Configuration["Token"];

    if (token == null)
        throw new Exception("Отсутствует токен");

    var handler = new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        EnableMultipleHttp2Connections = true,
        UseCookies = false,
        UseProxy = false,

        ConnectTimeout = TimeSpan.FromSeconds(30),

        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(15),
        KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always
    };

    var httpClient = new HttpClient(handler)
    {
        Timeout = TimeSpan.FromSeconds(120)
    };

    return new Telegram.Bot.TelegramBotClient(token, httpClient);
});

var app = builder.Build();

var botHost = app.Services.GetRequiredService<BotHost>();
_ = Task.Run(() => botHost.RunAsync()); 

app.Run();
