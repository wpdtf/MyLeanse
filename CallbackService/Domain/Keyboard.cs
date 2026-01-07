using Telegram.Bot.Types.ReplyMarkups;

namespace MyLeanse.CallbackService.Domain;

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

    /// <summary>
    /// Клавиатура для блока с дополнительными командами
    /// </summary>
    public static InlineKeyboardMarkup KeyboardAboutInfo { get; } = new InlineKeyboardMarkup(
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
}
