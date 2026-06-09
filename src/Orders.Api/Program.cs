using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Interfaces;
using Orders.Api.Middlewares;
using Orders.Api.Repositories;
using Orders.Api.Services;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHttpClient<IStocksClient, StocksClient>(client =>
{
    var baseAddress = builder.Configuration["Services:StocksApi:BaseAddress"] ?? "http://localhost:5163";
    client.BaseAddress = new Uri(baseAddress);
});

builder.AddAzureServiceBusClient("messaging");
builder.Services.AddHostedService<PaymentConsumerService>();
builder.Services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.DarkMode = true);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
