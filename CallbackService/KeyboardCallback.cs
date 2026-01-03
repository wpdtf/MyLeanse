using Humanizer;
using MyLeanse.LocalDatabase;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyLeanse.CallbackService;

public class KeyboardCallback(LeanseStorage leanseStorage)
{
    private readonly LeanseStorage _leanseStorage = leanseStorage;

    public async Task ExecuteAsync(Update update)
    {
        if (update.CallbackQuery.Message.From.Id == 5608566941)
            switch (update.CallbackQuery.Data)
            {
                case "startLense":
                    {
                        await StartTimeAsync(update);
                        return;
                    }
                case "stopLense":
                    {
                        await EndTimeAsync(update);
                        return;
                    }
                case "aboutInfo":
                    {
                        await SendMessageAsync(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                $"Дополнительная информация",
                                Keyboard.KeyboardAboutInfo());
                        return;
                    }
                case "infoLeanse":
                    {
                        await MenuAsync(update);
                        return;
                    }
                case "createLeanse":
                    {
                        await ClearLeanseAsync(update);
                        return;
                    }
                case "menu":
                    {
                        await MenuAsync(update);
                        return;
                    }
            }
    }

    public async Task StartTimeAsync(Update update)
    {
        var result = _leanseStorage.IsActive();
        var message = "";
        var info = _leanseStorage.Info();

        if (result)
        {
            message = $"Линзы уже были надеты!\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
        }
        else
        {
            _leanseStorage.Start();
            message = $"Линзы надеты!\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
        }

        await SendMessageAsync(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                message,
                                Keyboard.KeyboardStart());
    }

    public async Task EndTimeAsync(Update update)
    {
        var result = _leanseStorage.End();
        var message = "";
        var info = _leanseStorage.Info();

        if (result)
        {
            message = $"Линзы сняты\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
        }
        else
        {
            message = $"Отсутствовали надетые линзы!\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
        }

        await SendMessageAsync(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                message,
                                Keyboard.KeyboardStart());
    }

    public async Task ClearLeanseAsync(Update update)
    {
        _leanseStorage.Clear();

        await SendMessageAsync(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                $"Линзы удалены",
                                Keyboard.KeyboardStart());
    }

    public async Task MenuAsync(Update update)
    {
        var info = _leanseStorage.Info();

        await SendMessageAsync(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                $"Меню\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}",
                                Keyboard.KeyboardStart());
    }

    private async Task SendMessageAsync(DateTime sendTime, ChatId chatId, int messageId, string oldMessage, string message, InlineKeyboardMarkup keyboard)
    {
        if (sendTime > DateTime.Now.AddHours(-12) && oldMessage != message)
        {
            await BotStatic._botClient.EditMessageText(chatId, messageId, message, replyMarkup: keyboard);
        }
        else
        {
            await BotStatic._botClient.SendMessage(chatId, message, replyMarkup: keyboard);
        }
    }
}
