using Ingredients.Protos;

namespace Ingredients.Tests;

public class CrustsTests : IClassFixture<IngredientsApplicationFactory>
{
    private readonly IngredientsApplicationFactory _factory;

    public CrustsTests(IngredientsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetsCrusts()
    {
        var request = new GetCrustsRequest();
        var client = _factory.CreateGrpcClient();
        var response = await client.GetCrustsAsync(request);
        Assert.NotEmpty(response.Crusts);
    }
}