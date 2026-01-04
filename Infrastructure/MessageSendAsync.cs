using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using MyLeanse.Infrastructure.Domain;
using static System.Net.Mime.MediaTypeNames;

namespace MyLeanse.Infrastructure;

public class MessageSendAsync
{
    private int _messagesSentThisSecond = 0;
    private DateTime _lastResetTime = DateTime.UtcNow;

    private List<string> listEmoje = new List<string>() { "😅", "😇", "🥰", "😍", "😏", "😊" };
    private Random rnd = new Random();

    public async Task CheckEditMessageText(DateTime sendTime, ChatId chatId, int messageId, string oldMessage, string message, InlineKeyboardMarkup keyboard)
    {
        if (sendTime > DateTime.Now.AddHours(-45))
        {
            if (oldMessage == message)
                message += "\n" + listEmoje[rnd.Next(listEmoje.Count)];

            await EditMessageText(chatId.Identifier.Value, messageId, message, keyboard);
        }
        else
        {
            await SendMessage(chatId.Identifier.Value, message, replyMarkup: keyboard);
        }
    }

    public async Task SendMessage(long chatId, string message, ParseMode parseMode = ParseMode.None, InlineKeyboardMarkup? replyMarkup = null)
    {
        await SendMessageWithRateLimit(BotStatic._botClient.SendMessage(chatId, message, parseMode, replyMarkup: replyMarkup), 20);
    }

    public async Task EditMessageText(long chatId, int messageId, string newText, InlineKeyboardMarkup? replyMarkup = null)
    {
        await SendMessageWithRateLimit(BotStatic._botClient.EditMessageText(chatId, messageId, newText, replyMarkup: replyMarkup), 20);
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
