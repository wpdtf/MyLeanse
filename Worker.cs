namespace MyLeanse;

public class Worker (ILogger<Worker> logger, BotService botService) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly BotService _botService = botService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await _botService.BotStartAsync();
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
