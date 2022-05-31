using Ingredients.Data;
using Ingredients.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
        k.ListenLocalhost(5002, l =>
        {
            l.Protocols = HttpProtocols.Http2;
        });
    });
}

builder.Services.AddSingleton<IToppingData, ToppingData>();
builder.Services.AddSingleton<ICrustData, CrustData>();
builder.Services.AddSingleton<IngredientsImpl>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<IngredientsImpl>();

app.Run();
