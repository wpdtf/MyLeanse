using MyLeanse.Handlers.Domain;
using MyLeanse.Infrastructure;
using MyLeanse.Infrastructure.Interface;
using Telegram.Bot.Types;

namespace MyLeanse.Handlers.CommandHandler;

public class StartCommandHandler(ILogger<StartCommandHandler> logger, MessageSendAsync sendAsync) : ICommandHandler
{
    public string Command => "/start";
    private readonly ILogger<StartCommandHandler> _logger = logger;
    private readonly MessageSendAsync _sendAsync = sendAsync;

    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        await _sendAsync.SendMessage(
            message.Chat.Id,
            "Привет! 😘\n" +
            "Я тут, чтобы помочь тебе следить за тем, когда ты надевал и снимал контактные линзы.\n" +
            "Просто нажимай кнопки снизу: «Надел линзы» или «Снял линзы», и я покажу, сколько всего времени ты их носил.",
            ct,
            replyMarkup: Keyboard.KeyboardMain);

        _logger.LogDebug("Command = {Command}", Command);
    }
}
