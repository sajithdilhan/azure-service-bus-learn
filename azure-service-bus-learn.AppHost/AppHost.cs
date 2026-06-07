var builder = DistributedApplication.CreateBuilder(args);

var stocksApi = builder.AddProject<Projects.Stocks_Api>("stocks-api");

builder.AddProject<Projects.Orders_Api>("orders-api")
    .WithReference(stocksApi)
    .WaitFor(stocksApi)
    .WithEnvironment("Services__StocksApi__BaseAddress", "https+http://stocks-api");

builder.AddProject<Projects.Payments_Api>("payments-api");

builder.Build().Run();
