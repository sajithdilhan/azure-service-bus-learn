using Orders.Api.Interfaces;
using Orders.Api.Repositories;
using Orders.Api.Services;
using System.Text.Json.Serialization;

namespace Orders_Api.DependencyInjection;

public static class Services
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddHostedService<PaymentConsumerService>();

        services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddOpenApi();

        return services;
    }
}
