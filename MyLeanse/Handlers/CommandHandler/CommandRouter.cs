using MyLeanse.Infrastructure.Interface;
using Telegram.Bot.Types;

namespace MyLeanse.Handlers.CommandHandler;

public class CommandRouter
{
    private readonly Dictionary<string, ICommandHandler> _handlers;

    public CommandRouter(IEnumerable<ICommandHandler> handlers)
    {
        _handlers = handlers.ToDictionary(h => h.Command);
    }

    public async Task RouteAsync(Message message, CancellationToken ct)
    {
        var command = message.Text!.Split(' ')[0];

        if (_handlers.TryGetValue(command, out var handler))
            await handler.HandleAsync(message, ct);
    }
}
