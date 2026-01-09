using MyLeanse.Handlers.CallbackHandler;
using MyLeanse.Handlers.CommandHandler;
using MyLeanse.Infrastructure;
using Telegram.Bot.Types;

namespace MyLeanse.Handlers;

public class UpdateDispatcher(ILogger<UpdateDispatcher> logger, CommandRouter commandRouter, MessageHandler messageHandler, CallbackRouter callbackRouter)
{
    private readonly ILogger<UpdateDispatcher> _logger = logger;
    private readonly CommandRouter _commandRouter = commandRouter;
    private readonly MessageHandler _messageHandler = messageHandler;
    private readonly CallbackRouter _callbackRouter = callbackRouter;

    public async Task DispatchAsync(Update update, CancellationToken ct)
    {
        if (update.CallbackQuery != null)
        {
            _logger.LogDebug("inline-keyboard, UserId = {UserId}", update.CallbackQuery.Message!.From!.Id);
            await _callbackRouter.RouteAsync(update.CallbackQuery, ct);
            return;
        }

        if (update.Message?.Text?.StartsWith('/') == true)
        {
            _logger.LogDebug("command, UserId = {UserId}", update.Message!.From!.Id);
            await _commandRouter.RouteAsync(update.Message, ct);
            return;
        }

        if (update.Message != null)
        {
            _logger.LogDebug("other, UserId = {UserId}", update.Message!.From!.Id);
            await _messageHandler.HandleAsync(update.Message, ct);
        }
    }
}
