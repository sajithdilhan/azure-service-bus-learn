using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Interfaces;
using Orders.Api.Services;

namespace Orders_Api.DependencyInjection;

public static class Infrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpClient<IStocksClient, StocksClient>(client =>
        {
            var baseAddress = configuration["Services:StocksApi:BaseAddress"] ?? "http://localhost:5163";
            client.BaseAddress = new Uri(baseAddress);
        });

        return services;
    }
}
