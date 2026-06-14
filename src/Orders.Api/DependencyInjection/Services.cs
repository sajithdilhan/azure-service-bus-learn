using Microsoft.OpenApi;
using Orders.Api.Interfaces;
using Orders.Api.Repositories;
using Orders.Api.Services;
using System.Text.Json.Serialization;

namespace Orders.Api.DependencyInjection;

public static class Services
{
    private const string BearerSecurityScheme = "Bearer";

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddHostedService<PaymentConsumerService>();

        services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[BearerSecurityScheme] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT bearer token authentication. Enter the token without the 'Bearer' prefix."
                };

                document.Security ??= [];
                document.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(BearerSecurityScheme, document)] = []
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }
}
