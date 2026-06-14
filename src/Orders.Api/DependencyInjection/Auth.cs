using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Common;
using System.Security.Claims;
using System.Text;

namespace Orders.Api.DependencyInjection;

public static class Auth
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = configuration.GetSection("AuthSettings").Get<AuthSettings>();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = authSettings!.Issuer,
                ValidAudience = authSettings!.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings!.SecretKey)),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = Constants.RoleClaimType,
                ClockSkew = TimeSpan.Zero
            };
        });
        return services;
    }

    public static IServiceCollection AddAuthorizationWithRoles(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Constants.AdminPolicy, policy => policy.RequireRole(Constants.AdminRole))
            .AddPolicy(Constants.UserPolicy, policy => policy.RequireRole(Constants.UserRole))
            .AddPolicy(Constants.AdminOrUserPolicy, policy => policy.RequireRole(Constants.AdminRole, Constants.UserRole));
        return services;
    }
}
