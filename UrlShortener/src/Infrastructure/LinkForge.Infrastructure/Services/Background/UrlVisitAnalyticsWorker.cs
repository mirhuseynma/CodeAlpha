namespace LinkForge.Infrastructure.Services.Background;

public class UrlVisitAnalyticsWorker : BackgroundService
{
    private readonly IUrlVisitQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UrlVisitAnalyticsWorker> _logger;

    public UrlVisitAnalyticsWorker(IUrlVisitQueue queue, IServiceScopeFactory scopeFactory, ILogger<UrlVisitAnalyticsWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UrlVisitAnalyticsWorker is starting.");

        await foreach (var visit in _queue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                
                dbContext.UrlVisits.Add(visit);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving URL visit analytics.");
            }
        }
    }
}

