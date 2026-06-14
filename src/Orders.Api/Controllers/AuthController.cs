using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Orders_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GenerateToken(string username, string password)
    {
        // Demo-only hardcoded credentials. Real applications should validate users against an identity store.
        if (username == "admin" && password == "password")
        {
            var token = GenerateJwtToken(username, Constants.AdminRole);
            return Ok(new { Token = token });
        }

        if (username == "user" && password == "password")
        {
            var token = GenerateJwtToken(username, Constants.UserRole);
            return Ok(new { Token = token });
        }

        return Unauthorized();
    }

    private string GenerateJwtToken(string username, string role)
    {
        var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var authSettings = configuration.GetSection("AuthSettings").Get<AuthSettings>();
        if (authSettings is null)
        {
            throw new InvalidOperationException("AuthSettings are not configured.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(Constants.RoleClaimType, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = creds,
            Issuer = authSettings.Issuer,
            Audience = authSettings.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
