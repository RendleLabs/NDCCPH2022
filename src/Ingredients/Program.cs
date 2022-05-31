using Grpc.HealthCheck;
using Ingredients;
using Ingredients.Data;
using Ingredients.Services;
using JaegerTracing;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var macOS = OperatingSystem.IsMacOS();

var builder = WebApplication.CreateBuilder(args);

builder.AddJaegerTracing();

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
builder.Services.AddSingleton<HealthServiceImpl>();
builder.Services.AddHostedService<HealthCheckBackgroundService>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<IngredientsImpl>();
app.MapGrpcService<HealthServiceImpl>();

app.Run();
