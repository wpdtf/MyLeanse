using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyLeanse.Infrastructure;

/// <summary>
/// Модель для отправки сообщений
/// </summary>
/// <remarks> учитываю ограничения на количество отправок в секунду </remarks>
public class MessageSendAsync(ITelegramBotClient bot)
{
    private readonly ITelegramBotClient _bot = bot;

    private int _messagesSentThisSecond = 0;
    private DateTime _lastResetTime = DateTime.UtcNow;

    private List<string> listEmoje = new List<string>() { "😅", "😇", "🥰", "😍", "😏", "😊" };
    private Random rnd = new Random();

    public async Task CheckEditMessageText(DateTime sendTime, long chatId, int messageId, string oldMessage, string message, InlineKeyboardMarkup keyboard, CancellationToken ct)
    {
        if (sendTime > DateTime.Now.AddHours(-45))
        {
            if (oldMessage == message)
                message += "\n" + listEmoje[rnd.Next(listEmoje.Count)];

            await EditMessageText(chatId, messageId, message, ct, keyboard);
        }
        else
        {
            await SendMessage(chatId, message, ct, replyMarkup: keyboard);
        }
    }

    public async Task SendMessage(long chatId, string message, CancellationToken ct, ParseMode parseMode = ParseMode.None, InlineKeyboardMarkup? replyMarkup = null)
    {
        await SendMessageWithRateLimit(_bot.SendMessage(chatId, message, parseMode, replyMarkup: replyMarkup, cancellationToken: ct), 20);
    }

    public async Task EditMessageText(long chatId, int messageId, string newText, CancellationToken ct, InlineKeyboardMarkup? replyMarkup = null)
    {
        await SendMessageWithRateLimit(_bot.EditMessageText(chatId, messageId, newText, cancellationToken: ct, replyMarkup: replyMarkup), 20);
    }

    private async Task SendMessageWithRateLimit(Task sendMessage, int maxRequestPerSecond = 20)
    {
        lock (this)
        {
            var now = DateTime.UtcNow;
            if ((now - _lastResetTime).TotalSeconds >= 1)
            {
                _messagesSentThisSecond = 0;
                _lastResetTime = now;
            }

            if (_messagesSentThisSecond >= maxRequestPerSecond)
            {
                var waitTime = 1000 - (int)(now - _lastResetTime).TotalMilliseconds;
                if (waitTime > 0)
                    Thread.Sleep(waitTime);

                _messagesSentThisSecond = 0;
                _lastResetTime = DateTime.UtcNow;
            }

            _messagesSentThisSecond++;
        }

        try
        {
            await sendMessage;
        }
        catch (ApiRequestException ex) when (ex.ErrorCode == 429)
        {
            var retryAfter = ex.Parameters?.RetryAfter ?? 1;
            await Task.Delay(retryAfter * 1000);
        }
    }
}
