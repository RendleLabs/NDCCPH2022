using Ingredients.Protos;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orders.PubSub;
using Orders.Services;

var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var macOS = OperatingSystem.IsMacOS();

var builder = WebApplication.CreateBuilder(args);

if (runningInContainer)
{
    builder.WebHost.ConfigureKestrel(k =>
    {
        k.ConfigureEndpointDefaults(o => o.Protocols = HttpProtocols.Http2);
    });
}
else if (macOS)
{
    builder.WebHost.ConfigureKestrel(k =>
    {
        k.ListenLocalhost(5004, l =>
        {
            l.Protocols = HttpProtocols.Http2;
        });
    });
}

builder.Services.AddGrpc();


var binding = runningInContainer || macOS ? "http" : "https";

var defaultUri = OperatingSystem.IsMacOS() ? "http://localhost:5002" : "https://localhost:5003";

var ingredientsUri = builder.Configuration.GetServiceUri("ingredients", binding)
    ?? new Uri(defaultUri);

builder.Services.AddGrpcClient<IngredientsService.IngredientsServiceClient>(o =>
{
    o.Address = ingredientsUri;
});

builder.Services.AddOrderPubSub();

var app = builder.Build();

app.MapGrpcService<OrdersImpl>();

app.Run();
