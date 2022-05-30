using Grpc.Core;
using Ingredients.Protos;

namespace Ingredients.Services;

public class IngredientsImpl : IngredientsService.IngredientsServiceBase
{
    public override Task<GetToppingsResponse> GetToppings(GetToppingsRequest request, ServerCallContext context)
    {
        return base.GetToppings(request, context);
    }
}