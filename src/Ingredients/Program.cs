using Ingredients.Data;
using Ingredients.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

if (OperatingSystem.IsMacOS())
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
