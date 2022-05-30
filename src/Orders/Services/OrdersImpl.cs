using Grpc.Core;
using Orders.Protos;

namespace Orders.Services;

public class OrdersImpl : OrderService.OrderServiceBase
{
    public override Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
    {
        return base.PlaceOrder(request, context);
    }
}