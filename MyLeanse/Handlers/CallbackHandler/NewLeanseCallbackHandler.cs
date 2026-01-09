using MyLeanse.Handlers.Domain;
using MyLeanse.Infrastructure;
using MyLeanse.Infrastructure.Interface;
using MyLeanse.LocalDatabase;
using Telegram.Bot.Types;

namespace MyLeanse.Handlers.CallbackHandler;

public class NewLeanseCallbackHandler(ILogger<NewLeanseCallbackHandler> logger, MessageSendAsync sendAsync, LeanseStorage leanseStorage) : ICallbackHandler
{
    public string Action => "new";
    private readonly ILogger<NewLeanseCallbackHandler> _logger = logger;
    private readonly MessageSendAsync _sendAsync = sendAsync;
    private readonly LeanseStorage _leanseStorage = leanseStorage;

    public async Task HandleAsync(CallbackQuery query, string[] args, CancellationToken ct)
    {
        _leanseStorage.Clear(query.From.Id);

        await _sendAsync.CheckEditMessageText(
            query.Message!.Date,
            query.Message!.Chat.Id,
            query.Message.MessageId,
            query.Message!.Text ?? "",
            "Очистил историю ношения предыдущих линз",
            Keyboard.KeyboardMain,
            ct);

        _logger.LogDebug("action = {Action}", Action);
    }
}
