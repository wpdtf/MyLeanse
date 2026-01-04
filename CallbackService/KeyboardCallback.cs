using Humanizer;
using MyLeanse.CallbackService.Domain;
using MyLeanse.Infrastructure;
using MyLeanse.LocalDatabase;
using System.Globalization;
using Telegram.Bot.Types;

namespace MyLeanse.CallbackService;

public class KeyboardCallback(LeanseStorage leanseStorage, MessageSendAsync messageSendAsync)
{
    private readonly LeanseStorage _leanseStorage = leanseStorage;
    private readonly MessageSendAsync _messageSendAsync = messageSendAsync;

    public async Task ExecuteAsync(Update update)
    {
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
                    await _messageSendAsync.CheckEditMessageText(update.CallbackQuery.Message.Date.ToLocalTime(),
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
        var result = _leanseStorage.IsActive(update.CallbackQuery.From.Id);
        var message = "";
        var info = _leanseStorage.Info(update.CallbackQuery.From.Id);

        if (result)
        {
            message = $"Линзы уже были надеты!\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
        }
        else
        {
            _leanseStorage.Start(update.CallbackQuery.From.Id);
            message = $"Линзы надеты!\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
        }

        await _messageSendAsync.CheckEditMessageText(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                message,
                                Keyboard.KeyboardStart());
    }

    public async Task EndTimeAsync(Update update)
    {
        var result = _leanseStorage.End(update.CallbackQuery.From.Id);
        var message = "";
        var info = _leanseStorage.Info(update.CallbackQuery.From.Id);

        if (result)
        {
            message = $"Линзы сняты\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
        }
        else
        {
            message = $"Отсутствовали надетые линзы!\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}";
        }

        await _messageSendAsync.CheckEditMessageText(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                message,
                                Keyboard.KeyboardStart());
    }

    public async Task ClearLeanseAsync(Update update)
    {
        _leanseStorage.Clear(update.CallbackQuery.From.Id);

        await _messageSendAsync.CheckEditMessageText(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                $"Линзы удалены",
                                Keyboard.KeyboardStart());
    }

    public async Task MenuAsync(Update update)
    {
        var info = _leanseStorage.Info(update.CallbackQuery.From.Id);

        await _messageSendAsync.CheckEditMessageText(update.CallbackQuery.Message.Date.ToLocalTime(),
                                update.CallbackQuery.Message.Chat.Id,
                                update.CallbackQuery.Message.MessageId,
                                update.CallbackQuery.Message.Text,
                                $"Меню\nЛинзы используются: {info.Humanize(culture: new CultureInfo("ru-RU"), precision: 2)}",
                                Keyboard.KeyboardStart());
    }
}
