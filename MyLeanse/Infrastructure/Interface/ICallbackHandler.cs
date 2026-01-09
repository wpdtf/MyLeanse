using Telegram.Bot.Types;

namespace MyLeanse.Infrastructure.Interface;

public interface ICallbackHandler
{
    string Action { get; }

    Task HandleAsync(CallbackQuery query, string[] args, CancellationToken ct);
}
