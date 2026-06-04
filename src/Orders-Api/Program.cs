using Scalar.AspNetCore;
using Orders.Api.Data;
using Orders.Api.Interfaces;
using Orders.Api.Repositories;
using Orders.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<InMemoryOrdersDatabase>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

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
