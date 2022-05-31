using Grpc.Health.V1;
using Grpc.HealthCheck;
using Ingredients.Data;

namespace Ingredients;

public class HealthCheckBackgroundService : BackgroundService
{
    private readonly IToppingData _toppingData;
    private readonly HealthServiceImpl _healthServiceImpl;
    private readonly ILogger<HealthCheckBackgroundService> _logger;

    public HealthCheckBackgroundService(IToppingData toppingData, HealthServiceImpl healthServiceImpl,
        ILogger<HealthCheckBackgroundService> logger)
    {
        _toppingData = toppingData;
        _healthServiceImpl = healthServiceImpl;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var _ = await _toppingData.GetAsync(stoppingToken);
                _healthServiceImpl.SetStatus("ingredients", HealthCheckResponse.Types.ServingStatus.Serving);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                _healthServiceImpl.SetStatus("ingredients", HealthCheckResponse.Types.ServingStatus.NotServing);
            }
        }
    }
}