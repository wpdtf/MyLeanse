using Humanizer;
using MyLeanse.CallbackService.Domain;
using MyLeanse.Infrastructure;
using MyLeanse.LocalDatabase;
using System.Globalization;
using Telegram.Bot.Types;

namespace MyLeanse.CallbackService;

public class MessageCallback(LeanseStorage leanseStorage, MessageSendAsync messageSendAsync)
{
    private readonly LeanseStorage _leanseStorage = leanseStorage;
    private readonly MessageSendAsync _messageSendAsync = messageSendAsync;

    public async Task ExecuteAsync(Update update)
    {
        await DefaultAsync(update);
    }

    private async Task DefaultAsync(Update update)
    {
        var info = _leanseStorage.Info(update.Message.From.Id);

        await _messageSendAsync.SendMessage(update.Message.Chat.Id, $"Меню\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}", replyMarkup: Keyboard.KeyboardStart());
    }
}
