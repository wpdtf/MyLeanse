using Telegram.Bot.Types;

namespace MyLeanse.Infrastructure.Interface;

public interface ICommandHandler
{
    string Command { get; }

    Task HandleAsync(Message message, CancellationToken ct);
}
