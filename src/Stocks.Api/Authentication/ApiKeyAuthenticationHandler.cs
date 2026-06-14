using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Shared.Common;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Stocks.Api.Authentication;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Constants.ApiKeyHeaderName, out var extractedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API key was not provided."));
        }

        var authSettings = configuration.GetSection("AuthSettings").Get<AuthSettings>();
        var apiKey = authSettings?.SecretKey;

        if (string.IsNullOrWhiteSpace(apiKey) || extractedApiKey != apiKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "orders-api")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
