using MyLeanse.CallbackService;
using MyLeanse.Infrastructure.Domain;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MyLeanse.Infrastructure;

public class BotService(ILogger<BotService> logger, 
                        MessageCallback messageCallback, 
                        KeyboardCallback keyboardCallback,
                        IConfiguration configuration)
{
    private readonly ILogger<BotService> _logger = logger;
    private readonly MessageCallback _messageCallback = messageCallback;
    private readonly KeyboardCallback _keyboardCallback = keyboardCallback;
    private readonly IConfiguration _configuration = configuration;

    private ReceiverOptions _receiverOptions;

    public async Task BotStartAsync(HttpClient httpClient, CancellationToken cancellationToken)
    {
        var token = _configuration.GetValue<string>("Token");

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogError("Not token");

            return;
        }

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

        BotStatic._botClient = new TelegramBotClient(token, httpClient);
        
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates =
            [
                UpdateType.Message, UpdateType.CallbackQuery,
            ],
            DropPendingUpdates = true
        };

        BotStatic._botClient.StartReceiving(UpdateAsync, Error, _receiverOptions, cancellationToken);

        await WaitForCancellationAsync(cancellationToken);
    }

    private async Task WaitForCancellationAsync(CancellationToken cancellationToken)
    {
        try
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => tcs.SetResult(true));
            await tcs.Task;
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Bot service is stopping...");
        }
    }

    private async Task UpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        await _messageCallback.ExecuteAsync(update);
                        return;
                    }

                case UpdateType.CallbackQuery:
                    {
                        await _keyboardCallback.ExecuteAsync(update);
                        return;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
