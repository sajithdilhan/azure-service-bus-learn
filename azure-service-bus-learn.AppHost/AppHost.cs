var builder = DistributedApplication.CreateBuilder(args);



var serviceBus = builder.AddAzureServiceBus("messaging")
    .RunAsEmulator(emulator =>
    {
        emulator
            .WithHostPort(56720)
            .WithConfiguration(config =>
            {
                config["UserConfig"]!["Namespaces"]![0]!["Name"] = "sbemulatorns";
            });
    });

serviceBus.AddServiceBusQueue("payments-queue");
var cache = builder.AddRedis("cache").WithRedisCommander();

var stocksApi = builder.AddProject<Projects.Stocks_Api>("stocks-api");
builder.AddProject<Projects.Orders_Api>("orders-api")
    .WithReference(stocksApi)
    .WithReference(serviceBus)
    .WithReference(cache)
    .WaitFor(stocksApi)
    //.WaitFor(serviceBus)
    .WithEnvironment("Services__StocksApi__BaseAddress", "https+http://stocks-api");

builder.AddProject<Projects.Payments_Api>("payments-api")
    .WithReference(serviceBus);
    //.WaitFor(serviceBus);

builder.Build().Run();
