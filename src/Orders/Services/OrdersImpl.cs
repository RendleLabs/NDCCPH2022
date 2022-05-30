using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ingredients.Protos;
using Orders.Protos;
using Orders.PubSub;

namespace Orders.Services;

public class OrdersImpl : OrderService.OrderServiceBase
{
    private readonly IngredientsService.IngredientsServiceClient _ingredients;
    private readonly IOrderPublisher _orderPublisher;
    private readonly IOrderMessages _orderMessages;
    private readonly ILogger<OrdersImpl> _logger;

    public OrdersImpl(IngredientsService.IngredientsServiceClient ingredients,
        IOrderPublisher orderPublisher, IOrderMessages orderMessages, ILogger<OrdersImpl> logger)
    {
        _ingredients = ingredients;
        _orderPublisher = orderPublisher;
        _orderMessages = orderMessages;
        _logger = logger;
    }
    
    public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
    {
        try
        {
            await _ingredients.DecrementToppingsAsync(new DecrementToppingsRequest
            {
                ToppingIds = {request.ToppingIds}
            });
            await _ingredients.DecrementCrustsAsync(new DecrementCrustsRequest
            {
                CrustId = request.CrustId
            });

            var dueBy = DateTimeOffset.UtcNow.AddMinutes(45);

            await _orderPublisher.PublishOrder(request.CrustId, request.ToppingIds, dueBy);

            return new PlaceOrderResponse
            {
                DueBy = dueBy.ToTimestamp()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, e.Message, e));
        }
    }

    public override async Task Subscribe(SubscribeRequest request,
        IServerStreamWriter<OrderNotification> responseStream,
        ServerCallContext context)
    {
        var cancellationToken = context.CancellationToken;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var message = await _orderMessages.ReadAsync(cancellationToken);
                var notification = new OrderNotification
                {
                    CrustId = message.CrustId,
                    ToppingIds = {message.ToppingIds},
                    DueBy = message.Time.ToTimestamp(),
                };
                try
                {
                    await responseStream.WriteAsync(notification, cancellationToken);
                }
                catch
                {
                    await _orderPublisher.PublishOrder(message.CrustId, message.ToppingIds, message.Time);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}