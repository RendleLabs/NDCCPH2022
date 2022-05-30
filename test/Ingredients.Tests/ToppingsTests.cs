using Ingredients.Protos;

namespace Ingredients.Tests;

public class ToppingsTests : IClassFixture<IngredientsApplicationFactory>
{
    private readonly IngredientsApplicationFactory _factory;

    public ToppingsTests(IngredientsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetsToppings()
    {
        var request = new GetToppingsRequest();
        var client = _factory.CreateGrpcClient();
        var response = await client.GetToppingsAsync(request);
        Assert.NotEmpty(response.Toppings);
    }
}