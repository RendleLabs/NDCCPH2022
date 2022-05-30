using System.Diagnostics;
using Grpc.Core;
using Ingredients.Data;
using Ingredients.Protos;

namespace Ingredients.Services;

public class IngredientsImpl : IngredientsService.IngredientsServiceBase
{
    private readonly IToppingData _toppingData;
    private readonly ICrustData _crustData;

    public IngredientsImpl(IToppingData toppingData, ICrustData crustData)
    {
        _toppingData = toppingData;
        _crustData = crustData;
    }
    
    public override async Task<GetToppingsResponse> GetToppings(GetToppingsRequest request, ServerCallContext context)
    {
        var toppings = (await _toppingData.GetAsync(context.CancellationToken))
            .OrderBy(t => t.Id);

        var response = new GetToppingsResponse
        {
            Toppings =
            {
                toppings.Select(t => new Topping
                {
                    Id = t.Id,
                    Name = t.Name,
                    Price = t.Price
                })
            }
        };

        return response;
    }

    public override async Task<GetCrustsResponse> GetCrusts(GetCrustsRequest request, ServerCallContext context)
    {
        Debug.WriteLine("Foo");
        var crusts = await _crustData.GetAsync(context.CancellationToken);

        var response = new GetCrustsResponse
        {
            Crusts =
            {
                crusts.Select(c => new Crust
                {
                    Id = c.Id,
                    Name = c.Name,
                    Size = c.Size,
                    Price = c.Price
                })
            }
        };

        return response;
    }

    public override async Task<DecrementToppingsResponse> DecrementToppings(DecrementToppingsRequest request, ServerCallContext context)
    {
        foreach (var id in request.ToppingIds)
        {
            await _toppingData.DecrementStockAsync(id);
        }

        return DecrementToppingsResponse;
    }

    public override async Task<DecrementCrustsResponse> DecrementCrusts(DecrementCrustsRequest request, ServerCallContext context)
    {
        await _crustData.DecrementStockAsync(request.CrustId);

        return DecrementCrustsResponse;
    }

    private static readonly DecrementToppingsResponse DecrementToppingsResponse = new();
    private static readonly DecrementCrustsResponse DecrementCrustsResponse = new();
}