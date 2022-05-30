using Orders.Protos;
using ShopConsole;

var binding = OperatingSystem.IsMacOS() ? "http" : "https";
var defaultUri = OperatingSystem.IsMacOS() ? "http://localhost:5004" : "https://localhost:5005";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddGrpcClient<OrderService.OrderServiceClient>((provider, options) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var uri = configuration.GetServiceUri("orders", binding) ?? new Uri(defaultUri);
            options.Address = uri;
        });
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
