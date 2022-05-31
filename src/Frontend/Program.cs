using Ingredients.Protos;
using JaegerTracing;
using Orders.Protos;

var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var macOS = OperatingSystem.IsMacOS();

var builder = WebApplication.CreateBuilder(args);

builder.AddJaegerTracing();

// Add services to the container.
builder.Services.AddControllersWithViews();

var binding = runningInContainer || macOS ? "http" : "https";

var defaultIngredientsUri = macOS ? "http://localhost:5002" : "https://localhost:5003";
var defaultOrdersUri = macOS ? "http://localhost:5004" : "https://localhost:5005";

var ingredientsUri = builder.Configuration.GetServiceUri("ingredients", binding)
    ?? new Uri(defaultIngredientsUri);

Console.WriteLine($"Ingredients: {ingredientsUri.ToString()}");

var ordersUri = builder.Configuration.GetServiceUri("orders", binding)
                ?? new Uri(defaultOrdersUri);

builder.Services.AddGrpcClient<IngredientsService.IngredientsServiceClient>(o =>
{
    o.Address = ingredientsUri;
});

builder.Services.AddGrpcClient<OrderService.OrderServiceClient>(o =>
{
    o.Address = ordersUri;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
