namespace MyLeanse.Infrastructure;

//TODO: идея с воркером который бы при падении бота сразу его перезапускал, хайповая, но это и докер делать будет, переписать
public class Worker (ILogger<Worker> logger, BotService botService) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly BotService _botService = botService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            EnableMultipleHttp2Connections = true,
            UseCookies = false,
            UseProxy = false,

            ConnectTimeout = TimeSpan.FromSeconds(30),

            KeepAlivePingDelay = TimeSpan.FromSeconds(30),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(15),
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always
        };

        var httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(120)
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _botService.BotStartAsync(httpClient, stoppingToken);
            await Task.Delay(200, stoppingToken);
        }
    }
}
