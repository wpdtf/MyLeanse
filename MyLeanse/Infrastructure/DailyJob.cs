using MyLeanse.LocalDatabase;
using Quartz;

namespace MyLeanse.Infrastructure;

class DailyJob(ILogger<DailyJob> logger, LeanseStorage leanseStorage) : IJob
{
    private readonly ILogger<DailyJob> _logger = logger;
    private readonly LeanseStorage _leanseStorage = leanseStorage;

    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            _leanseStorage.Cleanup();
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка очистки данных: {Ex}", ex);
        }


        return Task.CompletedTask;
    }
}
