using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shared.Common;
using Stocks.Api.Authentication;
using Stocks.Api.Data;
using Stocks.Api.Interfaces;
using Stocks.Api.Middlewares;
using Stocks.Api.Repositories;
using Stocks.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        Constants.ApiKeyAuthenticationScheme,
        options => { });
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IStocksRepository, StocksRepository>();
builder.Services.AddScoped<IStocksService, StocksService>();
builder.Services.AddDbContext<StocksDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
    npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "stocks")));

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
