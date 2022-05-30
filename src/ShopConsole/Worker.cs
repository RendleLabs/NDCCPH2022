using Grpc.Core;
using Orders.Protos;

namespace ShopConsole;

public class Worker : BackgroundService
{
    private readonly OrderService.OrderServiceClient _orders;
    private readonly ILogger<Worker> _logger;

    public Worker( 
        OrderService.OrderServiceClient orders,
        ILogger<Worker> logger)
    {
        _orders = orders;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var call = _orders.Subscribe(new SubscribeRequest(), cancellationToken: stoppingToken);
                var stream = call.ResponseStream;

                await foreach (var notification in stream.ReadAllAsync(stoppingToken))
                {
                    _logger.LogInformation("Order: {CrustIds} with {ToppingIds} due by {DueBy}",
                        notification.CrustId,
                        string.Join(", ", notification.ToppingIds),
                        notification.DueBy.ToDateTimeOffset().ToLocalTime().ToString("t"));
                }
            }
            catch (OperationCanceledException)
            {
                if (stoppingToken.IsCancellationRequested) break;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}
