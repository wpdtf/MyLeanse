using Humanizer;
using MyLeanse.LocalDatabase;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyLeanse.CallbackService;

public class MessageCallback(LeanseStorage leanseStorage)
{
    private readonly LeanseStorage _leanseStorage = leanseStorage;

    public async Task ExecuteAsync(Update update)
    {
        if (update.Message.From.Id == 408663065)
            await DefaultAsync(update);
    }

    private async Task DefaultAsync(Update update)
    {
        var info = _leanseStorage.Info();

        await SendMessageAsync(update.Message.Chat.Id, $"Меню\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}", Keyboard.KeyboardStart());
    }

    private async Task SendMessageAsync(ChatId chatId, string message, InlineKeyboardMarkup keyboard)
        => await BotStatic._botClient.SendMessage(chatId, message, replyMarkup: keyboard);
}
