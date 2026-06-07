var builder = DistributedApplication.CreateBuilder(args);

var stocksApi = builder.AddProject<Projects.Stocks_Api>("stocks-api");

var serviceBus = builder.AddAzureServiceBus("messaging")
    .RunAsEmulator(emulator =>
    {
        emulator
            .WithHostPort(7777)
            .WithConfiguration(config =>
            {
                config["UserConfig"]!["Namespaces"]![0]!["Name"] = "sbemulatorns";
            });
    });

serviceBus.AddServiceBusQueue("payments-queue");

builder.AddProject<Projects.Orders_Api>("orders-api")
    .WithReference(stocksApi)
    .WithReference(serviceBus)
    .WaitFor(stocksApi)
    .WaitFor(serviceBus)
    .WithEnvironment("Services__StocksApi__BaseAddress", "https+http://stocks-api");

builder.AddProject<Projects.Payments_Api>("payments-api")
    .WithReference(serviceBus)
    .WaitFor(serviceBus);

builder.Build().Run();
