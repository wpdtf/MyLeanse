namespace MyLeanse.Infrastructure;

public class Worker (ILogger<Worker> logger, BotService botService) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly BotService _botService = botService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await _botService.BotStartAsync(httpClient, stoppingToken);
            }
            await Task.Delay(200, stoppingToken);
        }
    }
}
