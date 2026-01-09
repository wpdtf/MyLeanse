using Telegram.Bot.Types.ReplyMarkups;

namespace MyLeanse.Handlers.Domain;

/// <summary>
/// Класс со статичными моделями возможных вариаций клавиатур
/// </summary>
public class Keyboard
{
    /// <summary>
    /// Основаная клавиатура бота
    /// </summary>
    public static InlineKeyboardMarkup KeyboardMain { get; } = new InlineKeyboardMarkup(
        new List<InlineKeyboardButton[]>()
        {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Надел линзы 👁", "putOn"),
                    InlineKeyboardButton.WithCallbackData("Снял линзы 🐤", "takeOff"),
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Линзы используются? ❔", "status"),
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Дополнительно ➕", "about"),
                },

        });

    /// <summary>
    /// Клавиатура для блока с дополнительными командами
    /// </summary>
    public static InlineKeyboardMarkup KeyboardAboutInfo { get; } = new InlineKeyboardMarkup(
        new List<InlineKeyboardButton[]>()
        {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Теперь я использую новые линзы 🆕", "new"),
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "status"),
                },
        });
}
