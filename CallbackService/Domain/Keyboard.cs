using Telegram.Bot.Types.ReplyMarkups;

namespace MyLeanse.CallbackService.Domain;

class Keyboard
{
    public static InlineKeyboardMarkup KeyboardStart()
    {
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
        new List<InlineKeyboardButton[]>()
        {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Надел линзы 👁", "startLense"),
                    InlineKeyboardButton.WithCallbackData("Снял линзы 🐤", "stopLense"),
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Актуальная информация", "infoLeanse"),
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Дополнительно", "aboutInfo"),
                },

        });

        return inlineKeyboard;
    }

    public static InlineKeyboardMarkup KeyboardAboutInfo()
    {
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
        new List<InlineKeyboardButton[]>()
        {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Очистить историю", "createLeanse"),
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "menu"),
                },
        });

        return inlineKeyboard;
    }
}
