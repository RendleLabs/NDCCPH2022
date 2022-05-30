using Ingredients.Protos;

var builder = WebApplication.CreateBuilder(args);

var binding = OperatingSystem.IsMacOS() ? "http" : "https";
var defaultUri = OperatingSystem.IsMacOS() ? "http://localhost:5002" : "https://localhost:5003";

var ingredientsUri = builder.Configuration.GetServiceUri("ingredients", binding)
    ?? new Uri(defaultUri);

builder.Services.AddGrpcClient<IngredientsService.IngredientsServiceClient>(o =>
{
    o.Address = ingredientsUri;
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
