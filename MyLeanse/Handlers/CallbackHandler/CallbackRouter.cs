using MyLeanse.Infrastructure.Interface;
using Telegram.Bot.Types;

namespace MyLeanse.Handlers.CallbackHandler;

public class CallbackRouter
{
    private readonly Dictionary<string, ICallbackHandler> _handlers;

    public CallbackRouter(IEnumerable<ICallbackHandler> handlers)
    {
        _handlers = handlers.ToDictionary(h => h.Action);
    }

    public async Task RouteAsync(CallbackQuery query, CancellationToken ct)
    {
        var parts = query.Data!.Split(':');
        var action = parts[0];
        var args = parts.Skip(1).ToArray();

        if (_handlers.TryGetValue(action, out var handler))
            await handler.HandleAsync(query, args, ct);
    }
}
