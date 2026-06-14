using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Interfaces;
using Orders.Api.Services;
using Shared.Common;

namespace Orders.Api.DependencyInjection;

public static class Infrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "orders")));

        var authSettings = configuration.GetSection("AuthSettings").Get<AuthSettings>();
        if (authSettings is { SecretKey: null or "" })
            throw new InvalidOperationException("AuthSettings are not properly configured.");

        services.AddHttpClient<IStocksClient, StocksClient>(client =>
        {
            var baseAddress = configuration["Services:StocksApi:BaseAddress"] ?? "http://localhost:5163";
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add(Constants.ApiKeyHeaderName, authSettings!.SecretKey);
        });

        return services;
    }
}
