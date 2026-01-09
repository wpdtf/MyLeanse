using MyLeanse.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace MyLeanse.Infrastructure;

public class BotHost(ILogger<BotHost> logger, ITelegramBotClient bot, UpdateDispatcher dispatcher)
{
    private readonly ILogger<BotHost> _logger = logger;
    private readonly ITelegramBotClient _bot = bot;
    private readonly UpdateDispatcher _dispatcher = dispatcher;

    public async Task RunAsync()
    {
        using var cts = new CancellationTokenSource();

        _bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<Telegram.Bot.Types.Enums.UpdateType>()
            },
            cancellationToken: cts.Token);

        _logger.LogInformation("Bot started");

        await Task.Delay(Timeout.Infinite, cts.Token);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        _logger.LogDebug("New message");
        await _dispatcher.DispatchAsync(update, ct);
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Telegram error");
        return Task.CompletedTask;
    }
}
