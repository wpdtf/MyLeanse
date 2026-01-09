using Humanizer;
using MyLeanse.Handlers.Domain;
using MyLeanse.Infrastructure;
using MyLeanse.LocalDatabase;
using System.Globalization;
using Telegram.Bot.Types;

namespace MyLeanse.Handlers;

public class MessageHandler(ILogger<MessageHandler> logger, MessageSendAsync sendAsync, LeanseStorage leanseStorage)
{
    private readonly ILogger<MessageHandler> _logger = logger;
    private readonly MessageSendAsync _sendAsync = sendAsync;
    private readonly LeanseStorage _leanseStorage = leanseStorage;

    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        var text = GetTextMessage(message);

        await _sendAsync.SendMessage(
            message.Chat.Id,
            text,
            ct,
            replyMarkup: Keyboard.KeyboardMain);

        _logger.LogDebug("Send message = {Message}", text);
    }

    private string GetTextMessage(Message message)
    {
        var info = _leanseStorage.Info(message.From!.Id);

        if (info.Seconds <= 0)
            return "Меню\nЛинзы еще не использовались!";

        return $"Меню\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
    }
}
