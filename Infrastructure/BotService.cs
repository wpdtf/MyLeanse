using MyLeanse.CallbackService;
using MyLeanse.Infrastructure.Domain;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MyLeanse.Infrastructure;

/// <summary>
/// Сервис для старта бота
/// </summary>
/// <param name="logger">логирование</param>
/// <param name="messageCallback">Класс для обработки получения сообщений и т.п</param>
/// <param name="keyboardCallback">Класс для обработки получения событий клавиатур</param>
/// <param name="configuration">Настройки</param>
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
