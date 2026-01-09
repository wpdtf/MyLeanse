using Humanizer;
using MyLeanse.Handlers.Domain;
using MyLeanse.Infrastructure;
using MyLeanse.Infrastructure.Interface;
using MyLeanse.LocalDatabase;
using System.Globalization;
using Telegram.Bot.Types;

namespace MyLeanse.Handlers.CallbackHandler;

public class StatusCallbackHandler(ILogger<StatusCallbackHandler> logger, MessageSendAsync sendAsync, LeanseStorage leanseStorage) : ICallbackHandler
{
    public string Action => "status";
    private readonly ILogger<StatusCallbackHandler> _logger = logger;
    private readonly MessageSendAsync _sendAsync = sendAsync;
    private readonly LeanseStorage _leanseStorage = leanseStorage;

    public async Task HandleAsync(CallbackQuery query, string[] args, CancellationToken ct)
    {
        var info = _leanseStorage.Info(query.From.Id);
        var text = "";

        if (CheckPutOnLeanse(query.From.Id))
            text = "Линзы ещё используются!" + GetTextMessage(info);
        else
        {
            text = "Линзы в данный момент не используются!" + GetTextMessage(info);
        }

        await _sendAsync.CheckEditMessageText(
            query.Message!.Date,
            query.Message!.Chat.Id,
            query.Message.MessageId,
            query.Message!.Text ?? "",
            text,
            Keyboard.KeyboardMain,
            ct);

        _logger.LogDebug("action = {Action}, send message = {Text}", Action, text);
    }

    private bool CheckPutOnLeanse(long userId)
    {
        return _leanseStorage.IsActive(userId);
    }

    private string GetTextMessage(TimeSpan timeSpan)
    {
        if (timeSpan.Seconds <= 0)
            return "\nЛинзы еще не использовались!";

        return $"\nЛинзы используются: {timeSpan.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
    }
}
