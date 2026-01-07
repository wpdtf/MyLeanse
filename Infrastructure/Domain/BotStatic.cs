using Telegram.Bot;

namespace MyLeanse.Infrastructure.Domain;

/// <summary>
/// Статическая модель с данными бота
/// </summary>
public class BotStatic //TODO: статическая модель это какая-то хрень, когда=то надо сделать нормально
{
    public static ITelegramBotClient _botClient;
}
