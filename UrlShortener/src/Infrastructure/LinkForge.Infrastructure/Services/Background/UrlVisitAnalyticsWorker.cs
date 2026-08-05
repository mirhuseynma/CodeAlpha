using System.Net.Http;
using System.Net.Http.Json;

namespace LinkForge.Infrastructure.Services.Background;

public class UrlVisitAnalyticsWorker : BackgroundService
{
    private readonly IUrlVisitQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UrlVisitAnalyticsWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public UrlVisitAnalyticsWorker(
        IUrlVisitQueue queue, 
        IServiceScopeFactory scopeFactory, 
        ILogger<UrlVisitAnalyticsWorker> logger,
        IHttpClientFactory httpClientFactory)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UrlVisitAnalyticsWorker is starting.");

        var batch = new List<UrlVisitEventDto>();
        var batchSize = 50;
        var timeout = TimeSpan.FromSeconds(5);
        var lastFlushTime = DateTimeOffset.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(timeout);

                await foreach (var visit in _queue.DequeueAllAsync(cts.Token))
                {
                    batch.Add(visit);
                    
                    if (batch.Count >= batchSize)
                    {
                        break; // Process batch
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Timeout reached, flush what we have
            }

            if (batch.Count > 0)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                    
                    var entities = new List<UrlVisit>();
                    using var httpClient = _httpClientFactory.CreateClient();

                    foreach (var dto in batch)
                    {
                        string? country = null;
                        
                        if (!string.IsNullOrEmpty(dto.IpAddress) && dto.IpAddress != "unknown" && dto.IpAddress != "::1" && dto.IpAddress != "127.0.0.1")
                        {
                            try
                            {
                                var geoResponse = await httpClient.GetFromJsonAsync<GeoIpResponse>($"http://ip-api.com/json/{dto.IpAddress}", stoppingToken);
                                country = geoResponse?.Country;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to resolve GeoIP for {IpAddress}", dto.IpAddress);
                            }
                        }

                        entities.Add(new UrlVisit
                        {
                            ShortenedUrlId = dto.ShortenedUrlId,
                            IpAddress = dto.IpAddress,
                            UserAgent = dto.UserAgent,
                            Referer = dto.Referer,
                            Country = country ?? "Unknown"
                        });
                    }

                    dbContext.UrlVisits.AddRange(entities);
                    await dbContext.SaveChangesAsync(stoppingToken);
                    
                    batch.Clear();
                    lastFlushTime = DateTimeOffset.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while saving URL visit analytics batch.");
                }
            }
        }
    }

    private class GeoIpResponse
    {
        public string? Country { get; set; }
    }
}

