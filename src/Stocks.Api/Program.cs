using System.Text.Json.Serialization;
using Scalar.AspNetCore;
using Stocks.Api.Data;
using Stocks.Api.Interfaces;
using Stocks.Api.Repositories;
using Stocks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<InMemoryStocksDatabase>();
builder.Services.AddSingleton<IStocksRepository, StocksRepository>();
builder.Services.AddScoped<IStocksService, StocksService>();

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
