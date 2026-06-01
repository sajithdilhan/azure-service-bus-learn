var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Orders_Api>("orders-api");

builder.AddProject<Projects.Stocks_Api>("stocks-api");

builder.Build().Run();
