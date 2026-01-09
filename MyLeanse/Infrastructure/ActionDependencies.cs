using MyLeanse.Handlers.CallbackHandler;
using MyLeanse.Infrastructure.Interface;

namespace MyLeanse.Infrastructure;

public static class ActionDependencies
{
    public static IServiceCollection SetAction(this IServiceCollection services)
    {
        services.AddSingleton<ICallbackHandler, PutOnCallbackHandle>();
        services.AddSingleton<ICallbackHandler, TakeOffCallbackHandler>();
        services.AddSingleton<ICallbackHandler, StatusCallbackHandler>();
        services.AddSingleton<ICallbackHandler, AboutCallbackHandler>();
        services.AddSingleton<ICallbackHandler, NewLeanseCallbackHandler>();
        services.AddSingleton<CallbackRouter>();

        return services;
    }
}
