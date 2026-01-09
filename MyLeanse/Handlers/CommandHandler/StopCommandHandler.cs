using MyLeanse.Infrastructure;
using MyLeanse.Infrastructure.Interface;
using Telegram.Bot.Types;

namespace MyLeanse.Handlers.CommandHandler;

public class StopCommandHandler(ILogger<StopCommandHandler> logger, MessageSendAsync sendAsync) : ICommandHandler
{
    public string Command => "/stop";
    private readonly ILogger<StopCommandHandler> _logger = logger;
    private readonly MessageSendAsync _sendAsync = sendAsync;

    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        await _sendAsync.SendMessage(
            message.Chat.Id,
            "Возвращайтесь 😭",
            ct);

        _logger.LogDebug("Command = {Command}", Command);
    }
}
