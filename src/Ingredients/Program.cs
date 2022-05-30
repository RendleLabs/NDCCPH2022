using Ingredients.Data;
using Ingredients.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IToppingData, ToppingData>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<IngredientsImpl>();

app.Run();
