using MyLeanse.CallbackService;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MyLeanse;

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

    public async Task BotStartAsync()
    {
        var token = _configuration.GetValue<string>("Token");

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogError("Not token");

            return;
        }

        BotStatic._botClient = new TelegramBotClient(token);
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates =
            [
                UpdateType.Message, UpdateType.CallbackQuery,
            ]
        };

        BotStatic._botClient.StartReceiving(UpdateAsync, Error, _receiverOptions);

        _logger.LogInformation("Bot it`s start");

        await Task.Delay(-1);
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
