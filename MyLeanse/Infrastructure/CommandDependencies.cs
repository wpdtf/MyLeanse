using MyLeanse.Handlers.CommandHandler;
using MyLeanse.Infrastructure.Interface;

namespace MyLeanse.Infrastructure;

public static class CommandDependencies
{
    public static IServiceCollection SetCommand(this IServiceCollection services)
    {
        services.AddSingleton<ICommandHandler, StartCommandHandler>();
        services.AddSingleton<ICommandHandler, StopCommandHandler>();
        services.AddSingleton<CommandRouter>();

        return services;
    }
}
